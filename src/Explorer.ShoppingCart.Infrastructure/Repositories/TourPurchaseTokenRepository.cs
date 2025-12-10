using Explorer.ShoppingCart.Core.Domain;
using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.ShoppingCart.Infrastructure.Repositories
{
    public class TourPurchaseTokenRepository : ITourPurchaseTokenRepository
    {
        private readonly ToursContext _dbContext;

        public TourPurchaseTokenRepository(ToursContext dbContext)
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
