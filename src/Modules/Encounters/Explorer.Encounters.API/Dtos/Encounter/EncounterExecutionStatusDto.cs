namespace Explorer.Encounters.API.Dtos.EncounterExecution
{
    public class EncounterExecutionStatusDto
    {
        public bool IsCompleted { get; set; }
        public int SecondsInsideZone { get; set; }
        public int RequiredSeconds { get; set; }
        public string? CompletionTime { get; set; } 
    }
}
