using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class TourPreferencesDto
    {
        public long Id { get; set; }         
        public long TouristId { get; set; }

        public int PreferredDifficulty { get; set; } 
        public int WalkingScore { get; set; }
        public int BicycleScore { get; set; }
        public int CarScore { get; set; }
        public int BoatScore { get; set; }

        public List<string> Tags { get; set; } = new();
    }
}

