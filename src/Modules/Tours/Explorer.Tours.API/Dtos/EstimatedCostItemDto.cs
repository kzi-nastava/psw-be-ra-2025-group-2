namespace Explorer.Tours.API.Dtos;

public class EstimatedCostItemDto
{
    public int Category { get; set; }
    public string CategoryName { get; set; } = "";

    public decimal AmountPerPerson { get; set; }
    public string Currency { get; set; } = "";
    public string? Note { get; set; }

}