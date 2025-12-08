using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IPublicKeyPointRepository
{
    Task<PublicKeyPoint?> GetByIdAsync(long id);
    Task<IEnumerable<PublicKeyPoint>> GetByAuthorIdAsync(long authorId);
    Task<IEnumerable<PublicKeyPoint>> GetApprovedAsync();
    Task AddAsync(PublicKeyPoint publicKeyPoint);
    Task UpdateAsync(PublicKeyPoint publicKeyPoint);
    Task DeleteAsync(PublicKeyPoint publicKeyPoint);
    Task<PublicKeyPoint?> GetBySourceTourAndOrdinalNoAsync(long tourId, int ordinalNo);
}