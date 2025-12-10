namespace Explorer.Tours.API.Dtos;

public class PublicKeyPointRequestDto
{
    public long Id { get; set; }
    public long PublicKeyPointId { get; set; } 
    public long AuthorId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? RejectionReason { get; set; }
    public PublicKeyPointDto? PublicKeyPoint { get; set; } 
}