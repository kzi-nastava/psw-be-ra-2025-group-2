using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Encounters.Core.Domain
{
    public class TouristUnlockedFrame : Entity
    {
        public long UserId { get; private set; }
        public long FrameId { get; private set; }
        public DateTime UnlockedAt { get; private set; }
        
        protected TouristUnlockedFrame() { }

        public TouristUnlockedFrame(long userId, long frameId, DateTime unlockedAt)
        {
            if (userId <= 0) throw new ArgumentException("UserId must be > 0.");
            if (frameId <= 0) throw new ArgumentException("FrameId must be > 0.");

            UserId = userId;
            FrameId = frameId;
            UnlockedAt = unlockedAt;
        }
    }
}