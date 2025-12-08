using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum PublicKeyPointStatus
{
    Pending,    
    Approved,  
    Rejected    
}


public class PublicKeyPoint : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string SecretText { get; private set; }
    public string ImageUrl { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public long AuthorId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public long? SourceTourId { get; private set; }
    public int? SourceOrdinalNo { get; private set; }

    private PublicKeyPoint() { }

  
    public PublicKeyPoint(
        string name,
        string description,
        string secretText,
        string imageUrl,
        double latitude,
        double longitude,
        long authorId,
        long? sourceTourId = null,
        int? sourceOrdinalNo = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));
        if (authorId <= 0)
            throw new ArgumentException("AuthorId must be positive.", nameof(authorId));

        Name = name;
        Description = description;
        SecretText = secretText ?? string.Empty;
        ImageUrl = imageUrl ?? string.Empty;
        Latitude = latitude;
        Longitude = longitude;
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
        SourceTourId = sourceTourId;
        SourceOrdinalNo = sourceOrdinalNo;
    }

   
    public static PublicKeyPoint CreateFromKeyPoint(
        KeyPoint keyPoint,
        long authorId,
        long tourId)
    {
        return new PublicKeyPoint(
            keyPoint.Name,
            keyPoint.Description,
            keyPoint.SecretText,
            keyPoint.ImageUrl,
            keyPoint.Latitude,
            keyPoint.Longitude,
            authorId,
            tourId,
            keyPoint.OrdinalNo
        );
    }

 
    public KeyPoint ToKeyPoint(int ordinalNo)
    {
        return new KeyPoint(
            ordinalNo,
            Name,
            Description,
            SecretText,
            ImageUrl,
            Latitude,
            Longitude
        );
    }
}
