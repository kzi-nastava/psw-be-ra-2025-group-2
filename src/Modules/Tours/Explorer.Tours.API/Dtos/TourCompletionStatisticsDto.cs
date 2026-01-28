namespace Explorer.Tours.API.Dtos
{
    public class TourCompletionStatisticsDto
    {
        public long TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public int TotalPurchases { get; set; }
        public int StartedCount { get; set; }
        public int Range0To25 { get; set; }
        public int Range26To50 { get; set; }
        public int Range51To75 { get; set; }
        public int Range76To100 { get; set; }
        public double AverageCompletionPercentage { get; set; }
    }

    public class AuthorTourStatisticsDto
    {
        public List<TourCompletionStatisticsDto> TourStatistics { get; set; } = new();
        public OverallStatisticsDto OverallStatistics { get; set; } = new();
    }

    public class OverallStatisticsDto
    {
        public int TotalPurchases { get; set; }
        public int TotalStarted { get; set; }
        public int Range0To25 { get; set; }
        public int Range26To50 { get; set; }
        public int Range51To75 { get; set; }
        public int Range76To100 { get; set; }
        public double AverageCompletionPercentage { get; set; }
    }
}
