using AutoMapper;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Public;
namespace Explorer.Stakeholders.Core.UseCases
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

        public List<TourPurchaseTokenDto> CompletePurchase(long touristId)
        {
            var cartResult = _shoppingCartService.GetCart(touristId);

            if (cartResult == null || !cartResult.Items.Any())
            {
                throw new Exception ("Shopping cart is empty.");
            }

            var tokens = new List<TourPurchaseTokenDto>();

            foreach (var item in cartResult.Items)
            {
                var token = new TourPurchaseToken(touristId, item.TourId);
                var saved = _repository.Create(token);

                tokens.Add(_mapper.Map<TourPurchaseTokenDto>(saved));
            }

            // nakon kupovine - isprazni korpu
            _shoppingCartService.ClearCart(touristId);
            return tokens;
        }
    }
}
