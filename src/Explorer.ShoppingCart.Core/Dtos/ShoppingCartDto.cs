using System.Collections.Generic;

namespace Explorer.ShoppingCart.Core.Dtos
{
    public class ShoppingCartDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}