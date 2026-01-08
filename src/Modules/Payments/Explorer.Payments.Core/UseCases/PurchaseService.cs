using AutoMapper;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
namespace Explorer.Stakeholders.Core.UseCases
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITourPurchaseTokenRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPaymentRecordRepository _paymentRecordRepository;


        public PurchaseService(
            IShoppingCartService shoppingCartService,
            ITourPurchaseTokenRepository repository,
            IMapper mapper,
            IPaymentRecordRepository paymentRecordRepository
)
        {
            _shoppingCartService = shoppingCartService;
            _repository = repository;
            _mapper = mapper;
            _paymentRecordRepository = paymentRecordRepository;

        }

        public List<TourPurchaseTokenDto> CompletePurchase(long touristId)
        {
            var cartResult = _shoppingCartService.GetCart(touristId);

            if (cartResult == null || !cartResult.Items.Any())
            {
                throw new InvalidOperationException("Shopping cart is empty.");

            }

            var tokens = new List<TourPurchaseTokenDto>();
            var now = DateTime.UtcNow;


            foreach (var item in cartResult.Items)
            {
                var record = new PaymentRecord(
                    touristId,
                    item.TourId,
                    item.Price.Amount,
                    now
                );
                _paymentRecordRepository.Create(record);

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
