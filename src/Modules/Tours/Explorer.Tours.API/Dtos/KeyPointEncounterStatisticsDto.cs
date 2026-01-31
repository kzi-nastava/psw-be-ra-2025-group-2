namespace Explorer.Tours.API.Dtos
{
    public class KeyPointEncounterStatisticsDto
    {
        public long TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public List<KeyPointStatDto> KeyPoints { get; set; } = new();
    }

    public class KeyPointStatDto
    {
        public long KeyPointId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OrdinalNo { get; set; }
        public int TouristsArrived { get; set; }
        public EncounterStatDto? Encounter { get; set; }
    }

    public class EncounterStatDto
    {
        public long EncounterId { get; set; }
        public string EncounterName { get; set; } = string.Empty;
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public double SuccessRate { get; set; }
    }
}