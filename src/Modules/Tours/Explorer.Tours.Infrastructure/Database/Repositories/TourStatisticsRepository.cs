using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourStatisticsRepository : ITourStatisticsRepository
    {
        private readonly ToursContext _db;

        public TourStatisticsRepository(ToursContext db) => _db = db;

        public void IncrementPurchases(long tourId, int delta = 1)
        {
            if (delta <= 0) throw new ArgumentOutOfRangeException(nameof(delta));

            _db.Database.ExecuteSqlInterpolated($@"
                UPDATE tours.""Tours""
                SET purchases_count = purchases_count + {delta}
                WHERE ""Id"" = {tourId};
            ");
        }

        public void IncrementStarts(long tourId, int delta = 1)
        {
            if (delta <= 0) throw new ArgumentOutOfRangeException(nameof(delta));

            _db.Database.ExecuteSqlInterpolated($@"
                UPDATE tours.""Tours""
                SET starts_count = starts_count + {delta}
                WHERE ""Id"" = {tourId};
            ");
        }
    }
}