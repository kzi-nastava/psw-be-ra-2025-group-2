using AutoMapper;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Payments.API.Public;
using Explorer.Payments.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Tours.API.Internal;

namespace Explorer.Stakeholders.Core.UseCases

{
    public class PurchaseService : IPurchaseService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITourPurchaseTokenRepository _repository;
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;
        private readonly IPaymentRecordRepository _paymentRecordRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IInternalTourService _internalTourService;

        public PurchaseService(
            IShoppingCartService shoppingCartService,
            ITourPurchaseTokenRepository repository,
            ICouponService couponService,
            IMapper mapper,
            IPaymentRecordRepository paymentRecordRepository,
            IWalletRepository walletRepository,
            IInternalTourService internalTourService)
        {
            _shoppingCartService = shoppingCartService;
            _repository = repository;
            _couponService = couponService;
            _mapper = mapper;
            _paymentRecordRepository = paymentRecordRepository;
            _walletRepository = walletRepository;
            _internalTourService = internalTourService;

        }

        public List<TourPurchaseTokenDto> CompletePurchase(long touristId, string? couponCode = null)
        {
            var cartResult = _shoppingCartService.GetCart(touristId);
            if (cartResult == null || !cartResult.Items.Any())
            {
                throw new Exception("Shopping cart is empty.");
            }

            OrderItemDto? itemToDiscount = null;
            CouponDto? coupon = null;

            // Kuponi
            if (!string.IsNullOrEmpty(couponCode))
            {
                coupon = _couponService.GetByCode(couponCode) ?? throw new KeyNotFoundException("Coupon not found");

                if (DateTime.UtcNow > coupon.ValidUntil)
                {
                    throw new InvalidOperationException("Coupon is expired");
                }

                if (coupon.TourId.HasValue)
                {
                    itemToDiscount = cartResult.Items.FirstOrDefault(i => i.TourId == coupon.TourId.Value);
                    if (itemToDiscount == null)
                    {
                        throw new InvalidOperationException("Coupon not applicable to any item in cart");
                    }
                }
                else
                {
                    itemToDiscount = cartResult.Items
                        .Where(i => i.AuthorId == coupon.AuthorId)
                        .OrderByDescending(i => i.Price.Amount)
                        .FirstOrDefault();

                    if (itemToDiscount == null)
                    {
                        throw new InvalidOperationException("No applicable items for coupon");
                    }
                }
            }

            var tokens = new List<TourPurchaseTokenDto>();
            var now = DateTime.UtcNow;
            //var total = cartResult.Items.Sum(i => (int)i.Price.Amount);

            decimal total = 0;

            foreach (var item in cartResult.Items)
            {
                decimal price = item.Price.Amount;

                if (itemToDiscount != null && coupon != null && item.TourId == itemToDiscount.TourId)
                {
                    price -= price * coupon.DiscountPercentage / 100m;
                }

                total += price;
            }

            var wallet = _walletRepository.GetByTouristId(touristId);
            if (wallet == null)
            {
                throw new InvalidOperationException("Wallet not found.");
            }

            wallet.SpendAdventureCoins((int)total);
            _walletRepository.Update(wallet);



            foreach (var item in cartResult.Items)
            {
                decimal finalPrice = item.Price.Amount;

                if (itemToDiscount != null && coupon != null && item.TourId == itemToDiscount.TourId)
                {
                    var discountAmount = finalPrice * coupon.DiscountPercentage / 100m;
                    finalPrice -= discountAmount;

           
                }

                var record = new PaymentRecord(
                    touristId,
                    item.TourId,
                    finalPrice,
                    now
                );
                _paymentRecordRepository.Create(record);

                _internalTourService.IncrementTourPurchaseCount(item.TourId);

                var token = new TourPurchaseToken(touristId, item.TourId);
                var saved = _repository.Create(token);
                tokens.Add(_mapper.Map<TourPurchaseTokenDto>(saved));
            }

            _shoppingCartService.ClearCart(touristId);

            return tokens;
        }
    }
}
