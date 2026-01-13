using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.Tours.API.Internal;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases
{
    public class InternalTourService : IInternalTourService
    {
        private readonly ITourRepository _tourRepository;

        public InternalTourService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public void IncrementTourPurchaseCount(long tourId)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result;
            if (tour != null)
            {
                tour.IncrementPurchaseCount();
                _tourRepository.UpdateAsync(tour).Wait();
            }
        }
    }
}