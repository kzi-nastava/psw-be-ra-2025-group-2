using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterExecution : Entity
    {
        public long UserId { get; private set; }
        public long EncounterId { get; private set; }
        public DateTime? CompletionTime { get; private set; }
        public bool IsCompleted { get; private set; }
        public int XpAwarded { get; private set; }
        protected EncounterExecution() { } 

        public EncounterExecution(long userId, long encounterId)
        {
            UserId = userId;
            EncounterId = encounterId;
            IsCompleted = false;
            CompletionTime = null;
            XpAwarded = 0;
        }

        public void MarkCompleted(int xp)
        {
            if (IsCompleted) return;

            IsCompleted = true;
            CompletionTime = DateTime.UtcNow;
            XpAwarded = xp;
        }
    }
}