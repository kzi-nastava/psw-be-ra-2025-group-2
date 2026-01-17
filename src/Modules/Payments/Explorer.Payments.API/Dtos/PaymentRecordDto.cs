using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Payments.API.Dtos
{
    public class PaymentRecordDto
    {
        public long Id { get; set; }
        public long TouristId { get;  set; }
        public long? TourId { get; set; }
        public long? BundleId { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
