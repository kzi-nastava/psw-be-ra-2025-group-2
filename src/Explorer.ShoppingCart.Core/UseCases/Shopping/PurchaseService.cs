using AutoMapper;
using Explorer.ShoppingCart.Core.Domain;
using Explorer.ShoppingCart.Core.Dtos;
using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.Tours.Core.Domain;
using FluentResults;

namespace Explorer.ShoppingCart.Core.UseCases.Shopping
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITourPurchaseTokenRepository _repository;
        private readonly IMapper _mapper;

        public PurchaseService(
            IShoppingCartService shoppingCartService,
            ITourPurchaseTokenRepository repository,
            IMapper mapper)
        {
            _shoppingCartService = shoppingCartService;
            _repository = repository;
            _mapper = mapper;
        }

        public Result<List<TourPurchaseTokenDto>> CompletePurchase(long touristId)
        {
            var cartResult = _shoppingCartService.GetByTouristId(touristId);

            if (cartResult.IsFailed || cartResult.Value == null || !cartResult.Value.Items.Any())
            {
                return Result.Fail("Shopping cart is empty.");
            }

            var tokens = new List<TourPurchaseTokenDto>();

            foreach (var item in cartResult.Value.Items)
            {
                var token = new TourPurchaseToken(touristId, item.TourId);
                var saved = _repository.Create(token);

                tokens.Add(_mapper.Map<TourPurchaseTokenDto>(saved));
            }

            // nakon kupovine - isprazni korpu
            _shoppingCartService.ClearCart(touristId);
            return Result.Ok(tokens);
        }
    }
}
