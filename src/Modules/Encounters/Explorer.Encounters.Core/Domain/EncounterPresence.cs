using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class EncounterPresence : Entity
    {
        public long EncounterId { get; private set; }
        public long UserId { get; private set; }
        public DateTime LastSeenAt { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public EncounterPresence(long encounterId, long userId, double lat, double lon)
        {
            EncounterId = encounterId;
            UserId = userId;
            Update(lat, lon);
        }

        public void Update(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
            LastSeenAt = DateTime.UtcNow;
        }

        private EncounterPresence() { }
    }
}
