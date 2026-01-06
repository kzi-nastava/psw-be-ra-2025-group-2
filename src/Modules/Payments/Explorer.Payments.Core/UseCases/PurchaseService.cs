using AutoMapper;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;
using Explorer.Payments.API.Public;
using Explorer.Payments.API.Dtos;
namespace Explorer.Stakeholders.Core.UseCases
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITourPurchaseTokenRepository _repository;
        private readonly ICouponService _couponService;
        private readonly IMapper _mapper;

        public PurchaseService(
            IShoppingCartService shoppingCartService,
            ITourPurchaseTokenRepository repository,
            ICouponService couponService,
            IMapper mapper)
        {
            _shoppingCartService = shoppingCartService;
            _repository = repository;
            _couponService = couponService;
            _mapper = mapper;
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

            foreach (var item in cartResult.Items)
            {
                decimal finalPrice = item.Price.Amount;

                if (itemToDiscount != null && coupon != null && item.TourId == itemToDiscount.TourId)
                {
                    var discountAmount = finalPrice * coupon.DiscountPercentage / 100m;
                    finalPrice -= discountAmount;

           
                }

                var token = new TourPurchaseToken(touristId, item.TourId);
                var saved = _repository.Create(token);
                tokens.Add(_mapper.Map<TourPurchaseTokenDto>(saved));
            }

            _shoppingCartService.ClearCart(touristId);

            return tokens;
        }
    }
}
