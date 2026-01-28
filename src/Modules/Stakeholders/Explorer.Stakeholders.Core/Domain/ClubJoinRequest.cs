using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubJoinRequest : Entity
    {
        public long ClubId { get; private set; }
        public long TouristId { get; private set; }
        public DateTime RequestedAt { get; private set; }

        private ClubJoinRequest() { }

        public ClubJoinRequest(long clubId, long touristId)
        {
            ClubId = clubId;
            TouristId = touristId;
            RequestedAt = DateTime.UtcNow;
        }
    }
}