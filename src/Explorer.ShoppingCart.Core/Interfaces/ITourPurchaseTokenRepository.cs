using Explorer.ShoppingCart.Core.Domain;
using Explorer.Tours.Core.Domain;

namespace Explorer.ShoppingCart.Core.Interfaces
{
    public interface ITourPurchaseTokenRepository
    {
        TourPurchaseToken Create(TourPurchaseToken token);
        List<TourPurchaseToken> GetByTouristId(long touristId);
    }
}
