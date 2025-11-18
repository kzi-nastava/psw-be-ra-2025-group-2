using Explorer.BuildingBlocks.Core.Domain;
namespace Explorer.Stakeholders.Core.Domain;

public class Meetup : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }   // Markdown podrška je samo tekstualni tip
    public DateTime Date { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    // Ko je kreirao meetup (Autor ili Turista)
    public long CreatorId { get; private set; }

    public Meetup(string name, string description, DateTime date,
                  double latitude, double longitude, long creatorId)
    {
        Name = name;
        Description = description;
        Date = date;
        Latitude = latitude;
        Longitude = longitude;
        CreatorId = creatorId;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Invalid Name");

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Invalid Description");

        if (Date == default)
            throw new ArgumentException("Invalid Date");

        if (Latitude < -90 || Latitude > 90)
            throw new ArgumentException("Invalid Latitude");

        if (Longitude < -180 || Longitude > 180)
            throw new ArgumentException("Invalid Longitude");

        if (CreatorId <= 0)
            throw new ArgumentException("Invalid CreatorId");
    }
}