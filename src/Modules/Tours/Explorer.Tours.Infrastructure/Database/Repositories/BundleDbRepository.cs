using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class BundleDbRepository : IBundleRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<Bundle> _dbSet;

        public BundleDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Bundle>();
        }

        public Bundle Create(Bundle bundle)
        {
            _dbSet.Add(bundle);
            DbContext.SaveChanges();
            return bundle;
        }

        public Bundle GetById(long id)
        {
            return _dbSet.FirstOrDefault(b => b.Id == id);
        }

        public List<Bundle> GetByAuthorId(long authorId)
        {
            return _dbSet
                .Where(b => b.AuthorId == authorId)
                .ToList();
        }

        public Bundle Update(Bundle bundle)
        {
            DbContext.Update(bundle);
            DbContext.SaveChanges();
            return bundle;
        }

        public void Delete(long id)
        {
            var bundle = GetById(id);
            if (bundle != null)
            {
                _dbSet.Remove(bundle);
                DbContext.SaveChanges();
            }
        }

        public bool Exists(long id)
        {
            return _dbSet.Any(b => b.Id == id);
        }
    }
}
