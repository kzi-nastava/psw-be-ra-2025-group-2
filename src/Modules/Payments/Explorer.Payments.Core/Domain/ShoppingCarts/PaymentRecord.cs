using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.Core.Domain.ShoppingCarts
{
    public class PaymentRecord : Entity
    {
        public long TouristId { get; private set; }
        public long? TourId { get; private set; }
        public long? BundleId { get; private set; }
        public decimal Price { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // EF
        private PaymentRecord() { }

        public PaymentRecord(long touristId, long tourId, decimal price, DateTime createdAt)
        {
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));

            TouristId = touristId;
            TourId = tourId;
             BundleId = null;
            Price = price;
            CreatedAt = createdAt;
        }

        public PaymentRecord(long touristId, decimal price, DateTime createdAt, long bundleId)
        {
            if (touristId <= 0) throw new ArgumentOutOfRangeException(nameof(touristId));
            if (bundleId <= 0) throw new ArgumentOutOfRangeException(nameof(bundleId));
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));

            TouristId = touristId;
            TourId = null;
            BundleId = bundleId;
            Price = price;
            CreatedAt = createdAt;
        }

        
    }
}
