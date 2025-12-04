using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.Execution;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourExecutionDbRepository : ITourExecutionRepository
    {
        private readonly ToursContext _dbContext;

        public TourExecutionDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TourExecution Create(TourExecution execution)
        {
            _dbContext.TourExecutions.Add(execution);
            _dbContext.SaveChanges();
            return execution;
        }

        public void Delete(int id)
        {
            var entity = _dbContext.TourExecutions.Find(id);
            if (entity == null)
                throw new NotFoundException("Not found: " + id);

            _dbContext.TourExecutions.Remove(entity);
            _dbContext.SaveChanges();
        }

        public TourExecution Get(long id)
        {
            var entity = _dbContext.TourExecutions.Find(id);
            if (entity == null)
                throw new NotFoundException("Not found: " + id);

            return entity;
        }

        public TourExecution Update(TourExecution execution)
        {
            try
            {
                _dbContext.TourExecutions.Update(execution);
                _dbContext.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                throw new NotFoundException(ex.Message);
            }

            return execution;
        }
    }
}
