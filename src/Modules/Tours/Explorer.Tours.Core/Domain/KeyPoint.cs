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
       long authorId = 0,
       bool suggestForPublic = false)
    {
        OrdinalNo = ordinalNo;
        Name = name;
        Description = description;
        SecretText = secretText;
        ImageUrl = imageUrl;
        Latitude = latitude;
        Longitude = longitude;
        AuthorId = authorId;
        PublicStatus = suggestForPublic
            ? PublicPointRequestStatus.Pending
            : PublicPointRequestStatus.Private;

        Validate();
    }

    private void Validate()
    {
        if (OrdinalNo <= 0)
            throw new ArgumentException("OrdinalNo must be positive.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name cannot be empty.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty.");

        if (string.IsNullOrWhiteSpace(SecretText))
            throw new ArgumentException("SecretText cannot be empty.");

        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentException("Latitude must be between -90 and 90 degrees.");

        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentException("Longitude must be between -180 and 180 degrees.");
    }

    public void SetOrdinalNo(int newOrdinal)
    {
        if (newOrdinal <= 0)
            throw new ArgumentException("OrdinalNo must be positive.");

        OrdinalNo = newOrdinal;
    }

    public void SuggestForPublicUse()
    {
        if (PublicStatus == PublicPointRequestStatus.Pending)
            throw new InvalidOperationException("The request has already been sent.");

        if (PublicStatus == PublicPointRequestStatus.Approved)
            throw new InvalidOperationException("The key point is already public.");

        PublicStatus = PublicPointRequestStatus.Pending;
    }

    public void ApprovePublicRequest()
    {
        if (PublicStatus != PublicPointRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be approved.");

        PublicStatus = PublicPointRequestStatus.Approved;
    }

    public void RejectPublicRequest()
    {
        if (PublicStatus != PublicPointRequestStatus.Pending)
            throw new InvalidOperationException("Only pending requests can be denied.");

        PublicStatus = PublicPointRequestStatus.Rejected;
    }

    public void MakePrivate()
    {
        if (PublicStatus == PublicPointRequestStatus.Approved)
            throw new InvalidOperationException("Cannot remove public status. Contact the administrator.");

        PublicStatus = PublicPointRequestStatus.Private;
    }
}