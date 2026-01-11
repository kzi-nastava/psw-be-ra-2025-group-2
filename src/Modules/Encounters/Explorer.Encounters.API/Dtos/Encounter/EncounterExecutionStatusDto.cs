namespace Explorer.Encounters.API.Dtos.EncounterExecution
{
    public class EncounterExecutionStatusDto
    {
        public bool IsCompleted { get; set; }
        public int SecondsInsideZone { get; set; }
        public int RequiredSeconds { get; set; }
        public string? CompletionTime { get; set; }
        public int ActiveTourists { get; set; }
        public bool InRange { get; set; }
    }
}
