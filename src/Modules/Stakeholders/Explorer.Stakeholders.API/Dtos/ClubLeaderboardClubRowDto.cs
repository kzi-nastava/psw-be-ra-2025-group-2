using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubLeaderboardClubRowDto
    {
        public int Rank { get; set; }
        public long ClubId { get; set; }
        public string ClubName { get; set; } = "";
        public int MembersCount { get; set; }
        public int TotalXp { get; set; }
    }
}
