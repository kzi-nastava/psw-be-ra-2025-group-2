using System;

namespace Explorer.Tours.API.Dtos
{
    public class CartItemDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public int Quantity { get; set; }
    }
}
