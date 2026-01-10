using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos.TouristProgress
{
    public class TouristProgressDto
    {
        public long UserId { get; set; }
        public int TotalXp { get; set; }
        public int Level { get; set; }
        public bool CanCreateChallenges { get; set; }
    }
}

