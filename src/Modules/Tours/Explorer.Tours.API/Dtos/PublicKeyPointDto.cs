namespace Explorer.Tours.API.Dtos;

public class PublicKeyPointDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SecretText { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long AuthorId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public long? SourceTourId { get; set; }
    public int? SourceOrdinalNo { get; set; }
}