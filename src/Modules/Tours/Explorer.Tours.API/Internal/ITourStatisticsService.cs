using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Internal
{
    public interface ITourStatisticsService
    {
        void IncrementStarts(long tourId);
        void IncrementPurchases(long tourId);
    }
}
