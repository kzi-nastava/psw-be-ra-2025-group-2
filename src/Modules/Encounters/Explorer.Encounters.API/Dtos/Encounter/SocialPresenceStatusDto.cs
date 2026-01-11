using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos.Encounter
{
    public class SocialPresenceStatusDto
    {
        public bool InRange { get; set; }
        public int ActiveCount { get; set; }
        public int RequiredPeople { get; set; }
        public bool JustCompleted { get; set; }
    }
}
