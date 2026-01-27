using Explorer.Payments.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Statistics;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Statistics
{
    public class TourStatisticsService : ITourStatisticsService
    {
        private readonly ITourExecutionRepository _executionRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IInternalTokenService _tokenService;

        public TourStatisticsService(
            ITourExecutionRepository executionRepository,
            ITourRepository tourRepository,
            IInternalTokenService tokenService)
        {
            _executionRepository = executionRepository;
            _tourRepository = tourRepository;
            _tokenService = tokenService;
        }

        public TourCompletionStatisticsDto GetTourCompletionStatistics(long tourId)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result;
            if (tour == null)
                throw new InvalidOperationException($"Tour with id {tourId} not found.");

            var touristIds = _tokenService.GetTouristIdsByTourId(tourId).ToList();
            var executions = _executionRepository.GetByTourId(tourId);

            return CalculateTourStatistics(tourId, tour.Name, touristIds, executions);
        }

        public AuthorTourStatisticsDto GetAuthorTourStatistics(long authorId)
        {
            var tours = _tourRepository.GetByAuthorAsync(authorId).Result;
            var tourIds = tours.Select(t => t.Id).ToList();

            var allExecutions = _executionRepository.GetByTourIds(tourIds);
            var executionsByTour = allExecutions.GroupBy(e => e.TourId).ToDictionary(g => g.Key, g => g.ToList());

            var tourStatistics = new List<TourCompletionStatisticsDto>();

            foreach (var tour in tours)
            {
                var touristIds = _tokenService.GetTouristIdsByTourId(tour.Id).ToList();
                var tourExecutions = executionsByTour.GetValueOrDefault(tour.Id, new List<TourExecution>());

                var stats = CalculateTourStatistics(tour.Id, tour.Name, touristIds, tourExecutions);
                tourStatistics.Add(stats);
            }

            var overallStats = CalculateOverallStatistics(tourStatistics);

            return new AuthorTourStatisticsDto
            {
                TourStatistics = tourStatistics,
                OverallStatistics = overallStats
            };
        }

        private TourCompletionStatisticsDto CalculateTourStatistics(long tourId, string tourName, List<long> touristIds, List<TourExecution> executions)
        {
            var stats = new TourCompletionStatisticsDto
            {
                TourId = tourId,
                TourName = tourName,
                TotalPurchases = touristIds.Count
            };

            if (touristIds.Count == 0)
                return stats;

            var completionPercentages = new List<double>();

            foreach (var touristId in touristIds)
            {
                var latestExecution = executions
                    .Where(e => e.TouristId == touristId)
                    .OrderByDescending(e => e.LastActivityTimestamp)
                    .FirstOrDefault();

                double percentage = 0;
                if (latestExecution != null)
                {
                    stats.StartedCount++;
                    percentage = latestExecution.GetPercentageCompleted();
                }

                completionPercentages.Add(percentage);
                CategorizeCompletion(stats, percentage);
            }

            stats.AverageCompletionPercentage = Math.Round(completionPercentages.Average(), 2);
            return stats;
        }

        private void CategorizeCompletion(TourCompletionStatisticsDto stats, double percentage)
        {
            if (percentage <= 25)
                stats.Range0To25++;
            else if (percentage <= 50)
                stats.Range26To50++;
            else if (percentage <= 75)
                stats.Range51To75++;
            else
                stats.Range76To100++;
        }

        private OverallStatisticsDto CalculateOverallStatistics(List<TourCompletionStatisticsDto> tourStats)
        {
            var overall = new OverallStatisticsDto
            {
                TotalPurchases = tourStats.Sum(t => t.TotalPurchases),
                TotalStarted = tourStats.Sum(t => t.StartedCount),
                Range0To25 = tourStats.Sum(t => t.Range0To25),
                Range26To50 = tourStats.Sum(t => t.Range26To50),
                Range51To75 = tourStats.Sum(t => t.Range51To75),
                Range76To100 = tourStats.Sum(t => t.Range76To100)
            };

            if (overall.TotalPurchases > 0)
            {
                var totalWeightedAverage = tourStats
                    .Where(t => t.TotalPurchases > 0)
                    .Sum(t => t.AverageCompletionPercentage * t.TotalPurchases);
                overall.AverageCompletionPercentage = Math.Round(totalWeightedAverage / overall.TotalPurchases, 2);
            }

            return overall;
        }
    }
}
