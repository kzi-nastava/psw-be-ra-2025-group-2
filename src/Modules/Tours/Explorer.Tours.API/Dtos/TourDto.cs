using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public long AuthorId { get; private set; }
        public List<string> Tags { get; set; } = new();
        public string Status { get; set; }
        public decimal Price { get; set; }
        public decimal? LengthKm { get; set; }
        public List<KeyPointDto> KeyPoints { get; set; } = new();
        public List<TourDurationDto> Durations { get; set; } = new();
        public DateTime? PublishedAt { get; set; }

        public List<TourReviewDto> Reviews { get; set; }

        public bool IsActive { get; set; }
        public bool CanBeStarted { get; set; }

        public string? CoverImageUrl { get; set; }

        public int? EnvironmentType { get; set; }
        public int? AdventureLevel { get; set; }

        public List<int> SuitableFor { get; set; } = new();
        public List<int> FoodTypes { get; set; } = new();
        public List<int> ActivityTypes { get; set; } = new();

        public AverageCostDto? AverageCost { get; set; }


    }
}
