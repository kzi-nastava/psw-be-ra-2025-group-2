using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories 
{
    internal class TouristEquipmentDbRepository : ITouristEquipmentRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<TouristEquipment> _dbSet;

        public TouristEquipmentDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TouristEquipment>();
        }

        public TouristEquipment Create(TouristEquipment entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public void Delete(long id)
        {
           throw new NotImplementedException();
        }

        public TouristEquipment GetByUserId(long userId)
        {
            return _dbSet.Where(t => t.TouristId == userId).FirstOrDefault();
        }

        public TouristEquipment Update(TouristEquipment entity)
        {

            try
            {
                DbContext.Update(entity);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return entity;
        }
    }
}
