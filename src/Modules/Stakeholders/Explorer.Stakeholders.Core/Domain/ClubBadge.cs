using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubBadge : Entity
    {
        public long ClubId { get; private set; }
        public int MilestoneXp { get; private set; }   // 500, 1000, 1500...
        public DateTime AwardedAt { get; private set; }

        private ClubBadge() { }

        public ClubBadge(long clubId, int milestoneXp)
        {
            ClubId = clubId;
            MilestoneXp = milestoneXp;
            AwardedAt = DateTime.UtcNow;
        }
    }
}