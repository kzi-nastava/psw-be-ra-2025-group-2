namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IPublicKeyPointRequestRepository
{
    Task<PublicKeyPointRequest?> GetByIdAsync(long id);
    Task<IEnumerable<PublicKeyPointRequest>> GetByAuthorIdAsync(long authorId);
    Task<IEnumerable<PublicKeyPointRequest>> GetPendingRequestsAsync();
    Task AddAsync(PublicKeyPointRequest request);
    Task UpdateAsync(PublicKeyPointRequest request);
    Task<bool> ExistsPendingRequestAsync(long keyPointId);
    Task<Tour?> GetTourWithKeyPointAsync(long keyPointId);
    Task<PublicKeyPoint?> GetPublicKeyPointBySourceAsync(long tourId, int ordinalNo);
    Task AddPublicKeyPointAsync(PublicKeyPoint publicKeyPoint);
    Task<PublicKeyPoint?> GetPublicKeyPointByIdAsync(long id);
    Task UpdatePublicKeyPointAsync(PublicKeyPoint publicKeyPoint);
    Task<IEnumerable<PublicKeyPointRequest>> GetBySourceAsync(long tourId, int ordinalNo);
    Task DeleteAsync(PublicKeyPointRequest request);
    Task<PublicKeyPointRequest?> GetByPublicKeyPointIdAndStatusAsync(long publicKeyPointId, PublicKeyPointRequestStatus status);
}