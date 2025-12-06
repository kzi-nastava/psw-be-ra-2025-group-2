using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IShoppingCartService
{
    ShoppingCartDto GetCart(long touristId);
    ShoppingCartDto AddTourToCart(long touristId, long tourId, string tourName, double price, string category);
    ShoppingCartDto RemoveItemFromCart(long touristId, long itemId);
    void ClearCart(long touristId);
}