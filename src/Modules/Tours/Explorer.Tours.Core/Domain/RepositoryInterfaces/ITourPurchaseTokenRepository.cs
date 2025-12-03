using Explorer.Tours.Core.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITourPurchaseTokenRepository
    {
        Task<TourPurchaseToken> AddAsync(TourPurchaseToken token);
        Task<IEnumerable<TourPurchaseToken>> GetByUserId(long userId);
        Task<bool> Exists(long userId, long tourId);
    }
}