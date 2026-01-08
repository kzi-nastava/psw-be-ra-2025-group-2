using Explorer.BuildingBlocks.Core.Domain;
namespace Explorer.Stakeholders.Core.Domain.ShoppingCarts
{
    public class TourPurchaseToken : AggregateRoot
    {
        public long TouristId { get; private set; }
        public long TourId { get; private set; }
        public DateTime PurchaseDate { get; private set; }

        public TourPurchaseToken(long touristId, long tourId)
        {
            TouristId = touristId;
            TourId = tourId;
            PurchaseDate = DateTime.UtcNow;
        }
    }
}