using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class KeyPoint : Entity, IKeyPointInfo
{
    public int OrdinalNo { get; private set; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string SecretText { get; init; }
    public string ImageUrl { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public long AuthorId { get; private set; }
    public PublicPointRequestStatus PublicStatus { get; private set; }

    private KeyPoint() { }

    public KeyPoint(
        int ordinalNo,
        string name,
        string description,
        string secretText,
        string imageUrl,
        double latitude,
        double longitude,
        long authorId,
        bool suggestForPublic = false)
    {
        OrdinalNo = ordinalNo;
        Name = name;
        Description = description;
        SecretText = secretText ?? string.Empty;
        ImageUrl = imageUrl ?? string.Empty;
        Latitude = latitude;
        Longitude = longitude;
        AuthorId = authorId;

        PublicStatus = PublicPointRequestStatus.Private;


        Validate();
    }

    public KeyPoint(
        int ordinalNo,
        string name,
        string description,
        string secretText,
        string imageUrl,
        double latitude,
        double longitude)
        : this(ordinalNo, name, description, secretText, imageUrl, latitude, longitude, 0, false)
    {
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name is required");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description is required");

        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(Latitude));

        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(Longitude));
    }

    public void Update(string name, string description, string secretText, string imageUrl, double latitude, double longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required");

        typeof(KeyPoint).GetProperty(nameof(Name))!.SetValue(this, name);
        typeof(KeyPoint).GetProperty(nameof(Description))!.SetValue(this, description);
        typeof(KeyPoint).GetProperty(nameof(SecretText))!.SetValue(this, secretText ?? string.Empty);
        typeof(KeyPoint).GetProperty(nameof(ImageUrl))!.SetValue(this, imageUrl ?? string.Empty);
        typeof(KeyPoint).GetProperty(nameof(Latitude))!.SetValue(this, latitude);
        typeof(KeyPoint).GetProperty(nameof(Longitude))!.SetValue(this, longitude);


        Validate();
    }

    public void SetOrdinalNo(int newOrdinal)
    {
        if (newOrdinal < 1)
            throw new ArgumentOutOfRangeException(nameof(newOrdinal));

        OrdinalNo = newOrdinal;
    }

    public void SuggestForPublicUse()
    {
        if (PublicStatus == PublicPointRequestStatus.Pending)
            throw new InvalidOperationException("Request already submitted");

        if (PublicStatus == PublicPointRequestStatus.Approved)
            throw new InvalidOperationException("KeyPoint is already public");

        PublicStatus = PublicPointRequestStatus.Pending;
    }

    public void ApprovePublicRequest()
    {
        if (PublicStatus != PublicPointRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved");

        PublicStatus = PublicPointRequestStatus.Approved;
    }

    public void RejectPublicRequest()
    {
        if (PublicStatus != PublicPointRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be rejected");

        PublicStatus = PublicPointRequestStatus.Rejected;
    }

    public void MakePrivate()
    {
        if (PublicStatus == PublicPointRequestStatus.Approved)
            throw new InvalidOperationException("Cannot remove public status");

        PublicStatus = PublicPointRequestStatus.Private;
    }
}