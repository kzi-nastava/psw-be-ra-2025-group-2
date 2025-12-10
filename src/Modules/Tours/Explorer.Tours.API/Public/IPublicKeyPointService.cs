using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public;

public interface IPublicKeyPointService
{
    Task WithdrawRequestAsync(long keyPointId, long authorId);
    Task<IEnumerable<PublicKeyPointRequestDto>> GetAuthorRequestsAsync(long authorId);
    Task<IEnumerable<PublicKeyPointRequestDto>> GetPendingRequestsAsync();
    Task<PublicKeyPointRequestDto> ApproveRequestAsync(long requestId, long adminId);
    Task<PublicKeyPointRequestDto> SubmitRequestAsync(long tourId, int ordinalNo, long authorId);
    Task<PublicKeyPointRequestDto> RejectRequestAsync(long requestId, long adminId, string? reason);

}