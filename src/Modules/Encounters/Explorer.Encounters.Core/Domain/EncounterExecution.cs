using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterExecution : Entity
    {
        public long UserId { get; private set; }
        public long EncounterId { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? LastPingAt { get; private set; }
        public int SecondsInsideZone { get; private set; }
        public DateTime? CompletionTime { get; private set; }
        public bool IsCompleted { get; private set; }
        public int XpAwarded { get; private set; }
        public double LastLatitude { get; private set; }
        public double LastLongitude { get; private set; }
        public DateTime LastActivity { get; private set; }

        protected EncounterExecution() { }

        public EncounterExecution(long userId, long encounterId)
        {
            UserId = userId;
            EncounterId = encounterId;

            StartedAt = DateTime.UtcNow;
            LastPingAt = null;
            SecondsInsideZone = 0;

            IsCompleted = false;
            CompletionTime = null;
            XpAwarded = 0;

            LastLatitude = 0;
            LastLongitude = 0;
            LastActivity = DateTime.UtcNow;
        }

        public static EncounterExecution CreateCompleted(long userId, long encounterId)
        {
            var execution = new EncounterExecution(userId, encounterId);
            execution.MarkCompleted();
            return execution;
        }

        public void RegisterPing(bool isInsideZone, int deltaSeconds)
        {
            if (IsCompleted) return;

            if (deltaSeconds <= 0) deltaSeconds = 1;   // defensive
            if (deltaSeconds > 60) deltaSeconds = 60;  // defensive

            if (deltaSeconds <= 0) deltaSeconds = 1;
            if (deltaSeconds > 60) deltaSeconds = 60;

            LastPingAt = DateTime.UtcNow;

            if (!isInsideZone)
            {
                SecondsInsideZone = 0;
                return;
            }

            SecondsInsideZone += deltaSeconds;
        }

        public void MarkCompleted(int xpAwarded = 0)
        {
            if (IsCompleted) return;

            IsCompleted = true;
            CompletionTime = DateTime.UtcNow;
            XpAwarded = xpAwarded;
        }

        public void ResetProgress()
        {
            if (IsCompleted) return;
            SecondsInsideZone = 0;
        }
        
        public void UpdateLocation(double lat, double lon)
        {
            LastLatitude = lat;
            LastLongitude = lon;
            LastActivity = DateTime.UtcNow;
        }
    }
}
