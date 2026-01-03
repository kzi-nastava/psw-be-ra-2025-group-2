using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterExecution : Entity
    {
        public long UserId { get; private set; }
        public long EncounterId { get; private set; }
        public DateTime CompletionTime { get; private set; }
        public bool IsCompleted { get; private set; }

        public EncounterExecution(long userId, long encounterId)
        {
            UserId = userId;
            EncounterId = encounterId;
            CompletionTime = DateTime.UtcNow;
            IsCompleted = true; // Za Misc se odmah smatra završenim
        }
    }
}