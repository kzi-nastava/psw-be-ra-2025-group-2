using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Infrastructure.Database;
namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourPurchaseTokenDbRepository : ITourPurchaseTokenRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<TourPurchaseToken> _dbSet;

        public TourPurchaseTokenDbRepository(ToursContext context)
        {
            DbContext = context;
            _dbSet = DbContext.Set<TourPurchaseToken>();
        }

        public async Task<TourPurchaseToken> AddAsync(TourPurchaseToken token)
        {
            await _dbSet.AddAsync(token);
            await DbContext.SaveChangesAsync();
            return token;
        }

        public async Task<IEnumerable<TourPurchaseToken>> GetByUserId(long userId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> Exists(long userId, long tourId)
        {
            return await _dbSet.AnyAsync(t =>
                t.UserId == userId && t.TourId == tourId);
        }
    }
}