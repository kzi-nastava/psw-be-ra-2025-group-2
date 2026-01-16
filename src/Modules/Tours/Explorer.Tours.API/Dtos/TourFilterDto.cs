using System.Collections.Generic;
using System.Linq;

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
        
        // String properties za query params
        public string? SuitableFor { get; set; }
        public string? FoodTypes { get; set; }
        public string? AdventureLevel { get; set; }
        public string? ActivityTypes { get; set; }

        // Computed properties - konvertuju stringove u liste
        public List<int>? SuitableForList => ParseIntList(SuitableFor);
        public List<int>? FoodTypesList => ParseIntList(FoodTypes);
        public int? AdventureLevelValue => ParseNullableInt(AdventureLevel);
        public List<int>? ActivityTypesList => ParseIntList(ActivityTypes);

        private static List<int>? ParseIntList(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value.Split(',', System.StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => int.TryParse(s.Trim(), out int result) ? result : (int?)null)
                       .Where(i => i.HasValue)
                       .Select(i => i!.Value)
                       .ToList();
        }

        private static int? ParseNullableInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return int.TryParse(value.Trim(), out int result) ? result : (int?)null;
        }
    }
}   