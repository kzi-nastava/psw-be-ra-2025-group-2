using System;
using Explorer.BuildingBlocks.Core.Domain;
namespace Explorer.Tours.Core.Domain
{
    public class TourPurchaseToken : Entity
    {
        public long UserId { get; private set; }
        public long TourId { get; private set; }
        public DateTime PurchaseTime { get; private set; }

        public TourPurchaseToken() { }

        public TourPurchaseToken(long userId, long tourId)
        {
            if (userId == 0)
                throw new ArgumentException("UserId must not be equal to 0");

            if (tourId == 0)
                throw new ArgumentException("TourId must not be equal to 0");

            UserId = userId;
            TourId = tourId;
            PurchaseTime = DateTime.UtcNow;
        }
    }
}