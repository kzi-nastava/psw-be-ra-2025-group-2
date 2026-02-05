namespace Explorer.Tours.Core.Domain
{
    public record KeyPointUpdate(
        string? Name,
        string? Description,
        string? SecretText,
        string? ImageUrl,
        double Latitude,
        double Longitude,
        long? EncounterId,
        bool IsEncounterRequired,
        string? OsmClass,
        string? OsmType
    );
}