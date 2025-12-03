using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class TouristPositionDbRepository : ITouristPositionRepository
    {
        private readonly StakeholdersContext _dbContext;

        public TouristPositionDbRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TouristPosition CreateOrUpdate(TouristPosition position)
        {
            var existing = GetByTouristId(position.TouristId);

            if(existing == null)
            {
                _dbContext.Add(position);
            }
            else
            {
                existing.Update(position.TouristId, position.Latitude, position.Longitude);
                _dbContext.Update(existing);
            }
            _dbContext.SaveChanges();

            return position;
        }

        public TouristPosition? GetByTouristId(long touristId)
        {
            return _dbContext.TouristPositions.Where(p => p.TouristId == touristId).FirstOrDefault();
        }
    }
}
