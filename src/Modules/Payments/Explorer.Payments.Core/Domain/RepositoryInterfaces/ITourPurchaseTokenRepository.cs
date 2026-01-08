using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ITourPurchaseTokenRepository
    {
        TourPurchaseToken Create(TourPurchaseToken token);
        List<TourPurchaseToken> GetByTouristId(long touristId);
    }
}
