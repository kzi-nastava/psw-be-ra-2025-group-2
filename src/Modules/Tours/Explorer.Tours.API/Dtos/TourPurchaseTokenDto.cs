using System;

namespace Explorer.Tours.API.Dtos
{
    public class TourPurchaseTokenDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long TourId { get; set; }
        public DateTime PurchaseTime { get; set; }
    }
}