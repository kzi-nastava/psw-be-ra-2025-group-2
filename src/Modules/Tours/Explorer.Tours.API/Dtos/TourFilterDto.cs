using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos
{
    public class TourFilterDto
    {
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;

        // Filters
        public int? EnvironmentType { get; set; } // 1=Urban, 2=Nature, 3=Mixed
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public List<int>? SuitableFor { get; set; } // 1=Students, 2=Children, 3=Elderly, 4=Families, 5=Adults
        public List<int>? FoodTypes { get; set; } // 1=Vegetarian, 2=Vegan, 3=GlutenFree, etc.
        public int? AdventureLevel { get; set; } // 1=Low, 2=Medium, 3=High
        public List<int>? ActivityTypes { get; set; } // 1=Adrenaline, 2=Cultural, 3=Relaxing
    }
}