using Explorer.Tours.Core.Domain.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourReportRepository
    {
        TourReport GetById(long id);
        IEnumerable<TourReport> GetByTouristAndTour(long touristId, long tourId);
        TourReport Create(TourReport report);
        TourReport Update(TourReport report);
        void Delete(long id);
        IEnumerable<TourReport> GetPending();
    }
}
