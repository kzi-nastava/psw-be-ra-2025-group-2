using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Dtos;

public class ShoppingCartDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public MoneyDto TotalPrice { get; set; }
    public int ItemCount { get; set; }
}