using FluentResults;
using Explorer.ShoppingCart.Core.Domain;

namespace Explorer.ShoppingCart.Core.Interfaces;

public interface IShoppingCartService
{
    Result<Cart> GetByTouristId(long touristId);
    Task<Result<Cart>> AddItem(long touristId, long tourId);
    Result<Cart> RemoveItem(long touristId, long tourId);
    Result ClearCart(long touristId);
}