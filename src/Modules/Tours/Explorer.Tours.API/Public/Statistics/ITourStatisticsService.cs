using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Statistics
{
    public interface ITourStatisticsService
    {
        TourCompletionStatisticsDto GetTourCompletionStatistics(long tourId);
        AuthorTourStatisticsDto GetAuthorTourStatistics(long authorId);
        KeyPointEncounterStatisticsDto GetKeyPointEncounterStatistics(long tourId, long authorId);

    }
}
