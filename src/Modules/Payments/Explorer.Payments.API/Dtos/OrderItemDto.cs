
namespace Explorer.Stakeholders.API.Dtos;

public class OrderItemDto
{
    public long Id { get; set; }

    public long? TourId { get; set; }
    public long? BundleId { get; set; }
    public List<long> TourIds { get; set; } = new();

    public string ItemType { get; set; } = "TOUR";
    public string Title{ get; set; }
    public MoneyDto Price { get; set; }
    public long AuthorId { get; set; }
}