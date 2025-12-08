using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IPublicKeyPointRequestRepository
{
    Task<PublicKeyPointRequest?> GetByIdAsync(long id);
    Task<IEnumerable<PublicKeyPointRequest>> GetByAuthorIdAsync(long authorId);
    Task<IEnumerable<PublicKeyPointRequest>> GetPendingAsync();
    Task AddAsync(PublicKeyPointRequest request);
    Task UpdateAsync(PublicKeyPointRequest request);
    Task<bool> ExistsPendingForKeyPointAsync(long publicKeyPointId);
}