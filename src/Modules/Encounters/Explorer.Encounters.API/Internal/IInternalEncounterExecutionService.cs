namespace Explorer.Encounters.API.Internal
{
    public interface IInternalEncounterExecutionService
    {
        bool IsEncounterCompleted(long userId, long encounterId);
    }
}