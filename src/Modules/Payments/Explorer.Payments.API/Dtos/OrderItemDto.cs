using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Dtos;

public class OrderItemDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public string TourName { get; set; }
    public MoneyDto Price { get; set; }
    public long AuthorId { get; set; }
}