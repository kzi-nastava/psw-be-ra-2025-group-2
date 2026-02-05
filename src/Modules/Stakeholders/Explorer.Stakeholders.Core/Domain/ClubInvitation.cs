using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubInvitation : Entity
    {
        public long ClubId { get; private set; }
        public long TouristId { get; private set; }
        public DateTime SentAt { get; private set; }

        private ClubInvitation() { }

        public ClubInvitation(long clubId, long touristId)
        {
            ClubId = clubId;
            TouristId = touristId;
            SentAt = DateTime.UtcNow;
        }
    }
}
