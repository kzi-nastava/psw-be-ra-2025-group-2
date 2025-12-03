using System;
namespace Explorer.Tours.API.Dtos
{
    public class ShoppingCartDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public List<CartItemDto> Items { get; set; }
    }
}
