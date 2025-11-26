using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Stakeholders.Core.Domain;

public class TouristPosition : Entity
{
    public long PersonId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime RecordedAt { get; init; }

    public TouristPosition(long personId, double latitude, double longitude, DateTime recordedAt )
    {
        if (personId == 0) throw new ArgumentException("Invalid PersonId");
        if (latitude < -90 || latitude > 90) throw new ArgumentException("Invalid Latitude");
        if (longitude < -180 || longitude > 180) throw new ArgumentException("Invalid Longitude");

        PersonId = personId;
        Latitude = latitude;
        Longitude = longitude;
        RecordedAt = recordedAt ;
    }
}