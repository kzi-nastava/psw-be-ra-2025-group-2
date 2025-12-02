using Explorer.ShoppingCart.Core.Domain;

namespace Explorer.ShoppingCart.Core.Interfaces;

public interface IShoppingCartRepository
{
    Cart GetByTouristId(long touristId);
    Cart Create(Cart shoppingCart);
    Cart Update(Cart shoppingCart);
}