using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class PublishedTourPreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Difficulty { get; set; }
        public decimal Price { get; set; }
        public List<string> Tags { get; set; } = new();
        public KeyPointDto? FirstKeyPoint { get; set; }
        public List<TourReviewPublicDto> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public int KeyPointCount { get; set; }                 
        public int TotalDurationMinutes { get; set; }          
        public decimal? LengthKm { get; set; }                
        public string? PlaceName { get; set; }                

        public bool HasAuthorAward { get; set; }              
        public string? AuthorAwardName { get; set; }           

    }

    public class TourReviewPublicDto
    {
        public long Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
        public DateTime ReviewDate { get; set; }
        public float CompletedPercentage { get; set; }
        public List<string> Images { get; set; } = new();
        public long TouristId { get; set; }
        public string TouristName { get; set; } = "";
    }

}
