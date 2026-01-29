using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubLeaderboardRowDto
    {
        public int Rank { get; set; }
        public long TouristId { get; set; }
        public string Username { get; set; } = "";
        public int Level { get; set;  }
        public int TotalXp { get; set; }
    }
}
