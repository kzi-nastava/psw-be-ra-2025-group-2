using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class ClubDbRepository : IClubRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<Club> _dbSet;

        public ClubDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Club>();
        }

        public PagedResult<Club> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public Club Get(long id)
        {
            var entity = _dbSet
                .Include(c => c.Members)
                .Include(c => c.JoinRequests)
                .Include(c => c.Invitations)
                .SingleOrDefault(c => c.Id == id);

            if (entity == null)
                throw new NotFoundException("Club not found: " + id);

            return entity;
        }

        public List<Club> GetByOwner(long ownerId)
        {
            return _dbSet.Where(c => c.OwnerId == ownerId).ToList();
        }

        public Club Create(Club entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public Club Update(Club entity)
        {
            try
            {
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return entity;
        }

        public void Delete(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null)
                throw new NotFoundException("Club not found: " + id);

            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public List<Club> GetAll()
        {
            return _dbSet
                .Include(c => c.Members)
                .Include(c => c.JoinRequests)
                .Include(c => c.Invitations)
                .ToList();
        }

        public List<long> GetMemberClubIds(long touristId)
        {
            return _dbSet
                .Where(c => c.Members.Any(m => m.TouristId == touristId))
                .Select(c => c.Id)
                .ToList();
        }

        public List<long> GetMyJoinRequestClubIds(long touristId)
        {
            return _dbSet
                .Where(c => c.JoinRequests.Any(r => r.TouristId == touristId))
                .Select(c => c.Id)
                .ToList();
        }

        public List<long> GetInvitationClubIds(long touristId)
        {
            return _dbSet
                .Where(c => c.Invitations.Any(i => i.TouristId == touristId))
                .Select(c => c.Id)
                .ToList();
        }
    }
}
