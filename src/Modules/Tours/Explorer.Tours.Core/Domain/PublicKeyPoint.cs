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
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string SecretText { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public long AuthorId { get; private set; }
    public PublicKeyPointStatus Status { get; private set; }
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
        ValidateInputs(name, description, latitude, longitude);

        Name = name;
        Description = description;
        SecretText = secretText ?? string.Empty;
        ImageUrl = imageUrl ?? string.Empty;
        Latitude = latitude;
        Longitude = longitude;
        AuthorId = authorId;
        Status = PublicKeyPointStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        SourceTourId = sourceTourId;
        SourceOrdinalNo = sourceOrdinalNo;
    }

    public static PublicKeyPoint CreateFromKeyPoint(KeyPoint keyPoint, long authorId, long tourId)
    {
        if (keyPoint == null)
            throw new ArgumentNullException(nameof(keyPoint));

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

    public void Approve()
    {
        if (Status != PublicKeyPointStatus.Pending)
            throw new InvalidOperationException("Only pending keypoints can be approved.");

        Status = PublicKeyPointStatus.Approved;
    }

    public void Reject()
    {
        if (Status != PublicKeyPointStatus.Pending)
            throw new InvalidOperationException("Only pending keypoints can be rejected.");

        Status = PublicKeyPointStatus.Rejected;
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

    private void ValidateInputs(string name, string description, double latitude, double longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.", nameof(description));

        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
    }
}