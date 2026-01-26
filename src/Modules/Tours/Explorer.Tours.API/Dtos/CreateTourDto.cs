using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class CreateTourDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Difficulty { get; set; }
        public long AuthorId { get; set; }
        public decimal Price { get; set; } 

        public List<string>? Tags { get; set; }
        public List<KeyPointDto> KeyPoints { get; set; } = new();
        public List<TourDurationDto> Durations { get; set; } = new();
        public List<long> RequiredEquipmentIds { get; set; } = new();

        public int? EnvironmentType { get; set; }
        public int? AdventureLevel { get; set; }
        public List<int>? SuitableFor { get; set; }
        public List<int>? FoodTypes { get; set; }
        public List<int>? ActivityTypes { get; set; }

    }
}
