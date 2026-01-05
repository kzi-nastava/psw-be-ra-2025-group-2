using System.Text.Json.Serialization;

namespace Explorer.Encounters.API.Dtos.Encounter
{
    public class EncounterDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [JsonPropertyName("XP")]
        public int XP { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public int? RequiredPeople { get; set; }
        public double? Range { get; set; }
        public string? ImageUrl { get; set; }
        public double? ImageLatitude { get; set; }
        public double? ImageLongitude { get; set; }
        public double? DistanceTreshold { get; set; }
    }
}