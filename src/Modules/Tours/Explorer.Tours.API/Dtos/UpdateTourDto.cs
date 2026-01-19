using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class UpdateTourDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Difficulty { get; set; }
        public List<string>? Tags { get; set; }
        public decimal? LengthKm { get; set; }
        public decimal Price { get; set; }
        public List<KeyPointDto> KeyPoints { get; set; } = new();
        public List<TourDurationDto> Durations { get; set; } = new();
    }
}
