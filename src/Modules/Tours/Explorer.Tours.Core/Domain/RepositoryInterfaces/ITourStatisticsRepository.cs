using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourStatisticsRepository
    {
        void IncrementPurchases(long tourId, int delta = 1);
        void IncrementStarts(long tourId, int delta = 1);
    }
}
