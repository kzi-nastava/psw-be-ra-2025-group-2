namespace Explorer.Tours.API.Dtos;

public class EstimatedTourCostDto
{
    public decimal TotalPerPerson { get; set; }
    public string Currency { get; set; } = "";

    public bool IsInformational { get; set; } = true;

    public List<EstimatedCostItemDto> Breakdown { get; set; } = new();

    public string Disclaimer { get; set; } =
        "Iznos je procenjen na osnovu prosečnih cena i može varirati u zavisnosti od ličnih izbora, sezone i dostupnosti usluga.";
}
