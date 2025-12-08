using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface IPublicKeyPointService
{
    Task<PublicKeyPointDto> SubmitKeyPointForPublicUseAsync(long tourId, int ordinalNo, long authorId);

    Task<IEnumerable<PublicKeyPointDto>> GetAuthorPublicKeyPointsAsync(long authorId);

    Task<IEnumerable<PublicKeyPointRequestDto>> GetPendingRequestsAsync();

    Task<IEnumerable<PublicKeyPointDto>> GetApprovedPublicKeyPointsAsync();

    Task<PublicKeyPointDto> ApproveAsync(long publicKeyPointId, long adminId);

    Task<PublicKeyPointDto> RejectAsync(long publicKeyPointId, long adminId, string? reason);

    Task AddPublicKeyPointToTourAsync(long publicKeyPointId, long tourId, int ordinalNo, long authorId);
}