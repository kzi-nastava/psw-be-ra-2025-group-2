using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class AuthorAwardsDbRepository : IAuthorAwardsRepository
    {
        protected readonly StakeholdersContext DbContext;
        private readonly DbSet<AuthorAwards> _dbSet;

        public AuthorAwardsDbRepository(StakeholdersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<AuthorAwards>();
        }

        public AuthorAwards Create(AuthorAwards entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public void Delete(long id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null)
                throw new NotFoundException("Not found: " + id);

            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public bool ExistsByYear(int year)
        {
            return _dbSet.Any(x => x.Year == year);
        }

        public PagedResult<AuthorAwards> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public AuthorAwards Update(AuthorAwards entity)
        {
            try
            {
                DbContext.Update(entity);
                DbContext.SaveChanges();
            }
            catch(DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return entity;
        }
    }
}
