using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain.Execution;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourExecutionRepository
    {
        public TourExecution Get(long id);
        public TourExecution Create(TourExecution execution);
        public TourExecution Update(TourExecution execution);
        public void Delete(long id);
        public TourExecution? GetActiveByTouristId(long touristId);
        public TourExecution? GetExactExecution(long touristId, long tourId);
    }
}
