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
    public DateTime CreatedAt { get; set; }
    public long? SourceTourId { get; set; }
    public int? SourceOrdinalNo { get; set; }
    public string RequestStatus { get; set; } = "Pending";
    public DateTime? RequestCreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? RejectionReason { get; set; }
}


public class SubmitPublicKeyPointRequestDto
{
    public long TourId { get; set; }
    public int OrdinalNo { get; set; }
}

public class RejectPublicKeyPointDto
{
    public string? Reason { get; set; }
}

public class AddPublicKeyPointToTourDto
{
    public long PublicKeyPointId { get; set; }
    public long TourId { get; set; }
    public int OrdinalNo { get; set; }
}