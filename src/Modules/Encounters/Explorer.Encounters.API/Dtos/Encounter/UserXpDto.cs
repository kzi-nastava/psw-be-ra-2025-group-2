using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos.Encounter
{
    public class UserXpDto
    {
        public long UserId { get; set; }
        public int TotalXp { get; set; }
        public int Level { get; set; }
    }
}
