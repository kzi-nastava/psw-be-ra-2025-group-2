namespace Explorer.Tours.API.Dtos
{
    public class KeyPointEncounterDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int ExperiencePoints { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}