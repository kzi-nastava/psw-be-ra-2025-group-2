using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases;

public class PublicKeyPointService : IPublicKeyPointService
{
    private readonly IPublicKeyPointRequestRepository _requestRepository;
    private readonly ITourRepository _tourRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public PublicKeyPointService(
        IPublicKeyPointRequestRepository requestRepository,
        ITourRepository tourRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _tourRepository = tourRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<PublicKeyPointRequestDto> SubmitRequestAsync(
        long tourId,
        int ordinalNo,
        long authorId)
    {
        var tour = await GetTourWithKeyPointsOrThrow(tourId);
        var keyPoint = GetKeyPointFromTourOrThrow(tour, ordinalNo);
        var publicKeyPoint = await GetOrCreatePublicKeyPoint(tourId, ordinalNo, keyPoint, authorId);

        await ValidateNoPendingRequest(publicKeyPoint.Id);

        var request = await CreateAndSaveRequest(publicKeyPoint.Id, authorId);
        return _mapper.Map<PublicKeyPointRequestDto>(request);
    }

    public async Task<IEnumerable<PublicKeyPointRequestDto>> GetAuthorRequestsAsync(long authorId)
    {
        var requests = await _requestRepository.GetByAuthorIdAsync(authorId);
        return _mapper.Map<IEnumerable<PublicKeyPointRequestDto>>(requests);
    }

    public async Task<IEnumerable<PublicKeyPointRequestDto>> GetPendingRequestsAsync()
    {
        var requests = await _requestRepository.GetPendingRequestsAsync();
        return _mapper.Map<IEnumerable<PublicKeyPointRequestDto>>(requests);
    }

    public async Task<PublicKeyPointRequestDto> ApproveRequestAsync(long requestId, long adminId)
    {
        var request = await GetRequestOrThrow(requestId);
        ValidateRequestIsPending(request, "approved");

        var publicKeyPoint = await GetPublicKeyPointOrThrow(request.PublicKeyPointId);

        await ApprovePublicKeyPoint(publicKeyPoint);
        await ApproveRequest(request, adminId);
        await SendApprovalNotification(request.AuthorId, publicKeyPoint.Name);

        var updatedRequest = await _requestRepository.GetByIdAsync(requestId);
        return _mapper.Map<PublicKeyPointRequestDto>(updatedRequest);
    }

    public async Task<PublicKeyPointRequestDto> RejectRequestAsync(long requestId, long adminId, string? reason)
    {
        var request = await GetRequestOrThrow(requestId);
        ValidateRequestIsPending(request, "rejected");

        await RejectRequest(request, adminId, reason);
        await SendRejectionNotification(request.AuthorId, reason);

        var updatedRequest = await _requestRepository.GetByIdAsync(requestId);
        return _mapper.Map<PublicKeyPointRequestDto>(updatedRequest);
    }

    public Task WithdrawRequestAsync(long keyPointId, long authorId)
    {
        throw new NotImplementedException("Withdraw functionality is not yet implemented.");
    }

    // Private helper methods

    private async Task<Tour> GetTourWithKeyPointsOrThrow(long tourId)
    {
        var tour = await _tourRepository.GetTourWithKeyPointsAsync(tourId);
        if (tour == null)
            throw new KeyNotFoundException($"Tour with ID {tourId} not found.");

        return tour;
    }

    private static KeyPoint GetKeyPointFromTourOrThrow(Tour tour, int ordinalNo)
    {
        var keyPoint = tour.KeyPoints.FirstOrDefault(kp => kp.OrdinalNo == ordinalNo);
        if (keyPoint == null)
            throw new KeyNotFoundException($"KeyPoint with OrdinalNo {ordinalNo} not found in tour.");

        return keyPoint;
    }

    private async Task<PublicKeyPoint> GetOrCreatePublicKeyPoint(
        long tourId,
        int ordinalNo,
        KeyPoint keyPoint,
        long authorId)
    {
        var existingPublicKeyPoint = await _requestRepository
            .GetPublicKeyPointBySourceAsync(tourId, ordinalNo);

        if (existingPublicKeyPoint != null)
            return existingPublicKeyPoint;

        var newPublicKeyPoint = PublicKeyPoint.CreateFromKeyPoint(keyPoint, authorId, tourId);
        await _requestRepository.AddPublicKeyPointAsync(newPublicKeyPoint);

        return newPublicKeyPoint;
    }

    private async Task ValidateNoPendingRequest(long publicKeyPointId)
    {
        var hasPendingRequest = await _requestRepository.ExistsPendingRequestAsync(publicKeyPointId);
        if (hasPendingRequest)
            throw new InvalidOperationException("A pending request already exists for this keypoint.");
    }

    private async Task<PublicKeyPointRequest> CreateAndSaveRequest(long publicKeyPointId, long authorId)
    {
        var request = new PublicKeyPointRequest(publicKeyPointId, authorId);
        await _requestRepository.AddAsync(request);

        return await _requestRepository.GetByIdAsync(request.Id);
    }

    private async Task<PublicKeyPointRequest> GetRequestOrThrow(long requestId)
    {
        var request = await _requestRepository.GetByIdAsync(requestId);
        if (request == null)
            throw new KeyNotFoundException($"Request with ID {requestId} not found.");

        return request;
    }

    private static void ValidateRequestIsPending(PublicKeyPointRequest request, string action)
    {
        if (request.Status != PublicKeyPointRequestStatus.Pending)
            throw new InvalidOperationException($"Only pending requests can be {action}.");
    }

    private async Task<PublicKeyPoint> GetPublicKeyPointOrThrow(long publicKeyPointId)
    {
        var publicKeyPoint = await _requestRepository.GetPublicKeyPointByIdAsync(publicKeyPointId);
        if (publicKeyPoint == null)
            throw new KeyNotFoundException($"PublicKeyPoint with ID {publicKeyPointId} not found.");

        return publicKeyPoint;
    }

    private async Task ApprovePublicKeyPoint(PublicKeyPoint publicKeyPoint)
    {
        publicKeyPoint.Approve();
        await _requestRepository.UpdatePublicKeyPointAsync(publicKeyPoint);
    }

    private async Task ApproveRequest(PublicKeyPointRequest request, long adminId)
    {
        request.Approve(adminId);
        await _requestRepository.UpdateAsync(request);
    }

    private async Task SendApprovalNotification(long authorId, string keyPointName)
    {
        await _notificationService.NotifyAuthorApprovedAsync(authorId, keyPointName);
    }

    private async Task RejectRequest(PublicKeyPointRequest request, long adminId, string? reason)
    {
        request.Reject(adminId, reason);
        await _requestRepository.UpdateAsync(request);
    }

    private async Task SendRejectionNotification(long authorId, string? reason)
    {
        await _notificationService.NotifyAuthorRejectedAsync(authorId, "KeyPoint", reason);
    }
}