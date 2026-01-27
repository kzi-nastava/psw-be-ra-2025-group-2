using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;


namespace Explorer.Tours.Core.UseCases
{
    public class TourStatisticsService : ITourStatisticsService
    {
        private readonly ITourStatisticsRepository _repo;


        public TourStatisticsService(ITourStatisticsRepository repo)
        {
            _repo = repo;
        }


        public void IncrementStarts(long tourId) => _repo.IncrementStarts(tourId);


        public void IncrementPurchases(long tourId) => _repo.IncrementPurchases(tourId);
    }
}
