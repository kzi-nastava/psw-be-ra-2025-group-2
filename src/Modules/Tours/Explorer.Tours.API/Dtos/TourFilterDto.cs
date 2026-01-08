using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class TourFilterDto
    {
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;

        // Filters
        public int? EnvironmentType { get; set; } 
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SuitableFor { get; set; }
        public string? FoodTypes { get; set; } 
        public string? AdventureLevel { get; set; } 
        public string? ActivityTypes { get; set; } 
    }
}