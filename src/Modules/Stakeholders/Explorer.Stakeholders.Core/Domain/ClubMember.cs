using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubMember : Entity
    {
        public long TouristId { get; private set; }
        public DateTime JoinedAt { get; private set; }

        private ClubMember() { } 

        public ClubMember(long touristId)
        {
            TouristId = touristId;
            JoinedAt = DateTime.UtcNow;
        }
    }
}