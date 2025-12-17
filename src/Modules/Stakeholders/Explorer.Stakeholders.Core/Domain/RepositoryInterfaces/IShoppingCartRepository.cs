using Explorer.Stakeholders.Core.Domain.ShoppingCarts;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IShoppingCartRepository
{
    ShoppingCart? GetByTouristId(long touristId);
    ShoppingCart Create(ShoppingCart cart);
    ShoppingCart Update(ShoppingCart cart);
    void Delete(ShoppingCart cart);
}