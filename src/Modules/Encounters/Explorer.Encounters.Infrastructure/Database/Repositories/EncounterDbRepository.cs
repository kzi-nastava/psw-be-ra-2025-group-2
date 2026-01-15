using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterDbRepository : IEncounterRepository
    {
        private readonly EncountersContext _dbContext;

        public EncounterDbRepository(EncountersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Encounter Create(Encounter encounter)
        {
            _dbContext.Encounters.Add(encounter);
            _dbContext.SaveChanges();
            return encounter;
        }

        public void Delete(long id)
        {
            var entity = _dbContext.Encounters.Find(id);

            if(entity == null)
            {
                throw new NotFoundException($"Not found: {id}");
            }

            _dbContext.Encounters.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IEnumerable<Encounter> GetActive()
        {
            return _dbContext.Encounters.Where(e => e.State == EncounterState.Active);
        }

        public Encounter? GetById(long id)
        {
            return _dbContext.Encounters.Find(id);
        }

        public int GetCount()
        {
            return _dbContext.Encounters.Count();
        }

        public PagedResult<Encounter> GetPaged(int page, int pageSize)
        {
            var task = _dbContext.Encounters.GetPaged(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public Encounter Update(Encounter encounter)
        {
            try
            {
                _dbContext.Update(encounter);
                _dbContext.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                throw new NotFoundException(ex.Message);
            }
            return encounter;
        }
    }
}
