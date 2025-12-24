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
        public long TouristId { get; private set; }
        public DateTime RequestedAt { get; private set; }

        private ClubJoinRequest() { } 

        public ClubJoinRequest(long touristId)
        {
            TouristId = touristId;
            RequestedAt = DateTime.UtcNow;
        }
    }
}
