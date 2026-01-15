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
        public long TourId { get; private set; }
        public decimal Price { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // EF
        private PaymentRecord() { }

        public PaymentRecord(long touristId, long tourId, decimal price, DateTime createdAt)
        {
            if (price < 0) throw new ArgumentOutOfRangeException(nameof(price));

            TouristId = touristId;
            TourId = tourId;
            Price = price;
            CreatedAt = createdAt;
        }
    }
}
