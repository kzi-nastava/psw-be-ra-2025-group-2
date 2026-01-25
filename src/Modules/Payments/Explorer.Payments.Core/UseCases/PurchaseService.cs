using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWalletRepository _walletRepository;
        private readonly ICouponService _couponService;
        private readonly ITourStatisticsRepository _tourStats;

        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly ITourPurchaseTokenRepository _tourPurchaseTokenRepository;

        private readonly IMapper _mapper;

        public PurchaseService(
            IShoppingCartService shoppingCartService,
            IWalletRepository walletRepository,
            ICouponService couponService,
            IPaymentRecordRepository paymentRecordRepository,
            ITourPurchaseTokenRepository tourPurchaseTokenRepository,
            IMapper mapper,
            ITourStatisticsRepository tourStats)
        {
            _shoppingCartService = shoppingCartService;
            _walletRepository = walletRepository;
            _couponService = couponService;

            _paymentRecordRepository = paymentRecordRepository;
            _tourPurchaseTokenRepository = tourPurchaseTokenRepository;

            _mapper = mapper;
            _tourStats = tourStats;
        }

        public List<TourPurchaseTokenDto> CompletePurchase(long touristId, string? couponCode = null)
        {
            var cart = _shoppingCartService.GetCart(touristId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
                throw new InvalidOperationException("Shopping cart is empty.");

            OrderItemDto? itemToDiscount = null;
            CouponDto? coupon = null;

            if (!string.IsNullOrWhiteSpace(couponCode))
            {
                coupon = _couponService.GetByCode(couponCode)
                         ?? throw new KeyNotFoundException("Coupon not found");

                if (DateTime.UtcNow > coupon.ValidUntil)
                    throw new InvalidOperationException("Coupon is expired");

                // Kupon vezan za određenu turu
                if (coupon.TourId.HasValue)
                {
                    itemToDiscount = cart.Items.FirstOrDefault(i =>
                        i.TourId.HasValue && i.TourId.Value == coupon.TourId.Value);

                    if (itemToDiscount == null)
                        throw new InvalidOperationException("Coupon not applicable to any tour item in cart");
                }
                else
                {
                    // Kupon vezan za autora: diskontuj NAJSKUPLJU TURU tog autora
                    itemToDiscount = cart.Items
                        .Where(i => i.TourId.HasValue && i.AuthorId == coupon.AuthorId)
                        .OrderByDescending(i => i.Price.Amount)
                        .FirstOrDefault();

                    if (itemToDiscount == null)
                        throw new InvalidOperationException("No applicable tour items for coupon");
                }
            }

            decimal total = 0;

            foreach (var item in cart.Items)
            {
                decimal price = item.Price.Amount;

                var isDiscountedTour =
                    itemToDiscount != null &&
                    coupon != null &&
                    item.TourId.HasValue &&
                    itemToDiscount.TourId.HasValue &&
                    item.TourId.Value == itemToDiscount.TourId.Value;

                if (isDiscountedTour)
                {
                    price -= price * coupon!.DiscountPercentage / 100m;
                }

                total += price;
            }


            var wallet = _walletRepository.GetByTouristId(touristId)
                        ?? throw new InvalidOperationException("Wallet not found.");

            wallet.SpendAdventureCoins((int)total);
            _walletRepository.Update(wallet);

            var tokens = new List<TourPurchaseTokenDto>();
            var now = DateTime.UtcNow;

            foreach (var item in cart.Items)
            {
                decimal finalPrice = item.Price.Amount;

                var isDiscountedTour =
                    itemToDiscount != null &&
                    coupon != null &&
                    item.TourId.HasValue &&
                    itemToDiscount.TourId.HasValue &&
                    item.TourId.Value == itemToDiscount.TourId.Value;

                if (isDiscountedTour)
                {
                    finalPrice -= finalPrice * coupon!.DiscountPercentage / 100m;
                }

                if (item.TourId.HasValue)
                {
                    var record = new PaymentRecord(
                        touristId: touristId,
                        tourId: item.TourId.Value,
                        price: finalPrice,
                        createdAt: now
                    );
                    _paymentRecordRepository.Create(record);
                    _tourStats.IncrementPurchases(item.TourId.Value);

                    var token = new TourPurchaseToken(touristId, item.TourId.Value);
                    var savedToken = _tourPurchaseTokenRepository.Create(token);
                    tokens.Add(_mapper.Map<TourPurchaseTokenDto>(savedToken));

                    continue;
                }

                if (item.BundleId.HasValue)
                {
                    var record = new PaymentRecord(
                        touristId: touristId,
                        price: finalPrice,
                        createdAt: now,
                        bundleId: item.BundleId.Value
                    );
                    _paymentRecordRepository.Create(record);

                    if (item.TourIds != null && item.TourIds.Any())
                    {
                        foreach (var tid in item.TourIds)
                        {
                            if (tid <= 0) continue;
                            _tourStats.IncrementPurchases(tid);

                            var token = new TourPurchaseToken(touristId, tid);
                            var savedToken = _tourPurchaseTokenRepository.Create(token);
                            tokens.Add(_mapper.Map<TourPurchaseTokenDto>(savedToken));
                        }
                    }

                    continue;
                }

                
                throw new InvalidOperationException("Cart item has neither TourId nor BundleId.");
            }
            _shoppingCartService.ClearCart(touristId);

            return tokens;
        }

        public List<TourPurchaseTokenDto> CompleteBundlePurchase(long touristId, long bundleId)
        {
            throw new NotImplementedException("Use CompletePurchase via cart for simplest flow.");
        }
    }
}
