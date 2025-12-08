namespace Explorer.Tours.API.Dtos;

public class PublicKeyPointRequestDto
{
    public long RequestId { get; set; }
    public long PublicKeyPointId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SecretText { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long AuthorId { get; set; }
    public string RequestStatus { get; set; } = string.Empty;
    public DateTime RequestCreatedAt { get; set; }
    public long? SourceTourId { get; set; }
    public int? SourceOrdinalNo { get; set; }
}