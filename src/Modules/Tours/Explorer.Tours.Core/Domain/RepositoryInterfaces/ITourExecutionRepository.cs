using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain.TourSession;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourExecutionRepository
    {
        public TourExecution Get(int id);
        public TourExecution Create(TourExecution execution);
        public TourExecution Update(TourExecution execution);
        public void Delete(int id);
    }
}
