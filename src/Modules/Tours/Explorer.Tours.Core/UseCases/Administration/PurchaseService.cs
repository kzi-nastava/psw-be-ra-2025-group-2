using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Threading.Tasks;
using Explorer.Tours.API.Public.Administration;
namespace Explorer.Tours.Core.UseCases.Administration
{
    public class PurchaseService
    {
        private readonly ITourPurchaseTokenRepository _tokenRepository;
        private readonly IShoppingCartService _shoppingCartService;

        public PurchaseService(
            ITourPurchaseTokenRepository tokenRepository,
            IShoppingCartService shoppingCartService)
        {
            _tokenRepository = tokenRepository;
            _shoppingCartService = shoppingCartService;
        }

        public async Task Checkout(long userId)
        {
            var cart = await _shoppingCartService.GetCartForUser(userId);

            if (cart == null || cart.Items.Count == 0)
                throw new InvalidOperationException("Shopping cart is empty.");

            foreach (var item in cart.Items)
            {
                var alreadyPurchased = await _tokenRepository.Exists(userId, item.TourId);

                if (alreadyPurchased)
                    continue;

                var token = new TourPurchaseToken(userId, item.TourId);

                await _tokenRepository.AddAsync(token);
            }

            await _shoppingCartService.ClearCart(userId);
        }
    }
}