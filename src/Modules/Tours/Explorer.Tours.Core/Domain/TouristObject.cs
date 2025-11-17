using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;


public class TouristObject : Entity
{
    public string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public TouristObjectCategory Category { get; init; }

    public TouristObject(string name,double latitude,double longitude,TouristObjectCategory category)
    {
       
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
        Category = category;
        Validate();
    }
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Name cannot be empty.");
        if (Latitude < 0 || Latitude > 90) throw new ArgumentException("Latitude must be between 0 and 90 degrees.");
        if (Longitude < 0 || Longitude > 180) throw new ArgumentException("Longitude must be between 0 and 180 degrees.");
    }

}

public enum TouristObjectCategory
{
    WC,
    Restaurant,
    Parking,
    Other
}

