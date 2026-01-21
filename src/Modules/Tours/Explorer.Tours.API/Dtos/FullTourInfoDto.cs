using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class FullTourInfoDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Difficulty { get; set; }
        public decimal? LengthKm { get; set; }

        public int WalkingMinutes { get; set; }
        public int BicycleMinutes { get; set; }
        public int CarMinutes { get; set; }

        public List<EquipmentDto> Equipment { get; set; } = new();

        public double FirstKeyPointLatitude { get; set; }
        public double FirstKeyPointLongitude { get; set; }
    }
}
