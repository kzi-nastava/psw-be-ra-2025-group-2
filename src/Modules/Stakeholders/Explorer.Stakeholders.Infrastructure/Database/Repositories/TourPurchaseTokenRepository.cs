using Explorer.Stakeholders.Core.Domain.ShoppingCarts;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class TourPurchaseTokenRepository : ITourPurchaseTokenRepository
    {
        private readonly StakeholdersContext _dbContext;

        public TourPurchaseTokenRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TourPurchaseToken Create(TourPurchaseToken token)
        {
            _dbContext.TourPurchaseTokens.Add(token);
            _dbContext.SaveChanges();
            return token;
        }

        public List<TourPurchaseToken> GetByTouristId(long touristId)
        {
            return _dbContext.TourPurchaseTokens
                .Where(x => x.TouristId == touristId)
                .ToList();
        }
    }
}
