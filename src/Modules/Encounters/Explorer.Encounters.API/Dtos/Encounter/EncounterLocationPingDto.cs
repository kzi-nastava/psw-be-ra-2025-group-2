namespace Explorer.Encounters.API.Dtos.EncounterExecution
{
    public class EncounterLocationPingDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int? DeltaSeconds { get; set; }
    }
}