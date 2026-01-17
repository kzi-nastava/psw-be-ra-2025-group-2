using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IShoppingCartService
{
    ShoppingCartDto GetCart(long touristId);
    ShoppingCartDto AddTourToCart(long touristId, long tourId, string tourName, double price, string category, long authorId);

    ShoppingCartDto AddBundleToCart(long touristId, long bundleId, string bundleName, double price, List<long> tourIds, long authorId);

    ShoppingCartDto RemoveItemFromCart(long touristId, long itemId);
    void ClearCart(long touristId);
}