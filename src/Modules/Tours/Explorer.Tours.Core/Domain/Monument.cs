using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;
public class Monument : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int YearOfCreation { get; private set; }
    public MonumentState State { get; private set; }
    public float Latitude { get; private set; }
    public float Longitude { get; private set; }

    public Monument(string name, string description, int yearOfCreation, MonumentState state, float latitude, float longitude)
    {
        Name = name;
        Description = description;
        YearOfCreation = yearOfCreation;
        State = state;
        Latitude = latitude;
        Longitude = longitude;
        Validate();
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name.");
        if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description.");
        if (YearOfCreation > 2025) throw new ArgumentException("Invalid year of creation.");
        if (Latitude < -90 || Latitude > 90) throw new ArgumentException("Invalid Latitude.");
        if (Longitude < -180 || Longitude > 180) throw new ArgumentException("Invalid Longitude.");
    }
}

public enum MonumentState
{
    ACTIVE,
    INACTIVE
}
