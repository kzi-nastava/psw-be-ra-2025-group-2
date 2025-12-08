using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Runtime.Intrinsics.X86;

namespace Explorer.Tours.Core.UseCases.Administration;

public class PublicKeyPointService : IPublicKeyPointService
{
    private readonly IPublicKeyPointRepository _publicKeyPointRepository;
    private readonly IPublicKeyPointRequestRepository _requestRepository;
    private readonly ITourRepository _tourRepository;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;

    public PublicKeyPointService(
        IPublicKeyPointRepository publicKeyPointRepository,
        IPublicKeyPointRequestRepository requestRepository,
        ITourRepository tourRepository,
        INotificationService notificationService,
        IMapper mapper)
    {
        _publicKeyPointRepository = publicKeyPointRepository;
        _requestRepository = requestRepository;
        _tourRepository = tourRepository;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<PublicKeyPointDto> SubmitKeyPointForPublicUseAsync(long tourId, int ordinalNo, long authorId)
    {
        authorId = Math.Abs(authorId);

        var tour = await _tourRepository.GetByIdAsync(tourId)
            ?? throw new KeyNotFoundException("Tour not found.");

        if (Math.Abs(tour.AuthorId) != authorId)
            throw new UnauthorizedAccessException("You can only suggest your own key points.");

        var keyPoint = tour.KeyPoints.FirstOrDefault(kp => kp.OrdinalNo == ordinalNo)
            ?? throw new KeyNotFoundException("Key point not found.");

        var existing = await _publicKeyPointRepository
            .GetBySourceTourAndOrdinalNoAsync(tourId, ordinalNo);

        if (existing != null)
        {
            var hasPendingRequest = await _requestRepository
                .ExistsPendingForKeyPointAsync(existing.Id);

            if (hasPendingRequest)
                throw new InvalidOperationException("There is already a pending request for this key point.");

            var request = new PublicKeyPointRequest(existing.Id, authorId);
            await _requestRepository.AddAsync(request);

            return _mapper.Map<PublicKeyPointDto>(existing);
        }

        var publicKeyPoint = PublicKeyPoint.CreateFromKeyPoint(keyPoint, authorId, tourId);
        await _publicKeyPointRepository.AddAsync(publicKeyPoint);

        var newRequest = new PublicKeyPointRequest(publicKeyPoint.Id, authorId);
        await _requestRepository.AddAsync(newRequest);

        return _mapper.Map<PublicKeyPointDto>(publicKeyPoint);
    }


    public async Task<IEnumerable<PublicKeyPointDto>> GetAuthorPublicKeyPointsAsync(long authorId)
    {
        authorId = Math.Abs(authorId);

        var publicKeyPoints = await _publicKeyPointRepository.GetByAuthorIdAsync(authorId);
        var publicKeyPointIds = publicKeyPoints.Select(p => p.Id).ToList();

        var allRequests = await _requestRepository.GetByPublicKeyPointIdsAsync(publicKeyPointIds);

        var latestRequests = allRequests
            .GroupBy(r => r.PublicKeyPointId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(r => r.CreatedAt).First()
            );

        var result = publicKeyPoints.Select(pkp =>
        {
            var dto = _mapper.Map<PublicKeyPointDto>(pkp);

            if (latestRequests.TryGetValue(pkp.Id, out var request))
            {
                dto.RequestStatus = request.Status.ToString();
                dto.RequestCreatedAt = request.CreatedAt;
                dto.ProcessedAt = request.ProcessedAt;
                dto.RejectionReason = request.RejectionReason;
            }
            else
            {
                dto.RequestStatus = "Pending";
            }

            return dto;
        }).ToList();

        return result;
    }

    public async Task<IEnumerable<PublicKeyPointRequestDto>> GetPendingRequestsAsync()
    {
        var pendingRequests = await _requestRepository.GetPendingAsync();

        var result = pendingRequests
            .Where(r => r.PublicKeyPoint != null)
            .Select(r => new PublicKeyPointRequestDto
            {
                RequestId = r.Id,
                PublicKeyPointId = r.PublicKeyPointId,
                Name = r.PublicKeyPoint!.Name,
                Description = r.PublicKeyPoint.Description,
                SecretText = r.PublicKeyPoint.SecretText,
                ImageUrl = r.PublicKeyPoint.ImageUrl,
                Latitude = r.PublicKeyPoint.Latitude,
                Longitude = r.PublicKeyPoint.Longitude,
                AuthorId = r.PublicKeyPoint.AuthorId,
                RequestStatus = r.Status.ToString(), 
                RequestCreatedAt = r.CreatedAt,
                SourceTourId = r.PublicKeyPoint.SourceTourId,
                SourceOrdinalNo = r.PublicKeyPoint.SourceOrdinalNo
            })
            .ToList();

        return result;
    }

    public async Task<IEnumerable<PublicKeyPointDto>> GetApprovedPublicKeyPointsAsync()
    {
        var approved = await _publicKeyPointRepository.GetApprovedAsync();
        var approvedIds = approved.Select(p => p.Id).ToList();

        var allRequests = await _requestRepository.GetByPublicKeyPointIdsAsync(approvedIds);

        var latestRequests = allRequests
            .GroupBy(r => r.PublicKeyPointId)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(r => r.CreatedAt).First()
            );

        var result = approved.Select(pkp =>
        {
            var dto = _mapper.Map<PublicKeyPointDto>(pkp);

            if (latestRequests.TryGetValue(pkp.Id, out var request))
            {
                dto.RequestStatus = request.Status.ToString();
                dto.RequestCreatedAt = request.CreatedAt;
                dto.ProcessedAt = request.ProcessedAt;
                dto.RejectionReason = request.RejectionReason;
            }
            else
            {
                dto.RequestStatus = "Approved"; 
            }

            return dto;
        }).ToList();

        return result;
    }

    public async Task<PublicKeyPointDto> ApproveAsync(long publicKeyPointId, long adminId)
    {
        var request = (await _requestRepository.GetPendingAsync())
            .FirstOrDefault(r => r.PublicKeyPointId == publicKeyPointId)
            ?? throw new KeyNotFoundException("Request not found.");

        request.Approve(adminId);
        await _requestRepository.UpdateAsync(request);

        await _notificationService.NotifyAuthorApprovedAsync(
            Math.Abs(request.AuthorId),
            request.PublicKeyPoint.Name);

        var dto = _mapper.Map<PublicKeyPointDto>(request.PublicKeyPoint);
        dto.RequestStatus = request.Status.ToString();
        dto.RequestCreatedAt = request.CreatedAt;
        dto.ProcessedAt = request.ProcessedAt;
        dto.RejectionReason = request.RejectionReason;

        return dto;
    }
    public async Task<PublicKeyPointDto> RejectAsync(long publicKeyPointId, long adminId, string? reason)
    {
        adminId = Math.Abs(adminId);

        var publicKeyPoint = await _publicKeyPointRepository.GetByIdAsync(publicKeyPointId)
            ?? throw new KeyNotFoundException("Public key not found.");

        var request = (await _requestRepository.GetPendingAsync())
            .FirstOrDefault(r => r.PublicKeyPointId == publicKeyPointId)
            ?? throw new KeyNotFoundException("Request not found.");

        request.Reject(adminId, reason);
        await _requestRepository.UpdateAsync(request);

        await _notificationService.NotifyAuthorRejectedAsync(
            Math.Abs(publicKeyPoint.AuthorId),
            publicKeyPoint.Name,
            reason);

        var dto = _mapper.Map<PublicKeyPointDto>(publicKeyPoint);
        dto.RequestStatus = request.Status.ToString();
        dto.RequestCreatedAt = request.CreatedAt;
        dto.ProcessedAt = request.ProcessedAt;
        dto.RejectionReason = request.RejectionReason;

        return dto;
    }

    public async Task AddPublicKeyPointToTourAsync(long publicKeyPointId, long tourId, int ordinalNo, long authorId)
    {
        authorId = Math.Abs(authorId);

        var publicKeyPoint = await _publicKeyPointRepository.GetByIdAsync(publicKeyPointId)
            ?? throw new KeyNotFoundException("Public key point not found.");

        var approvedRequest = (await _requestRepository.GetByAuthorIdAsync(authorId))
            .FirstOrDefault(r => r.PublicKeyPointId == publicKeyPointId &&
                                r.Status == PublicKeyPointRequestStatus.Approved);

        if (approvedRequest == null)
            throw new InvalidOperationException("Only approved public points can be added to the tour.");

        var tour = await _tourRepository.GetByIdWithTrackingAsync(tourId)
            ?? throw new KeyNotFoundException("Tour not found.");

        if (Math.Abs(tour.AuthorId) != authorId)
            throw new UnauthorizedAccessException("You can only add points to your own tours.");

        var newKeyPoint = publicKeyPoint.ToKeyPoint(ordinalNo);
        tour.AddKeyPoint(newKeyPoint);

        await _tourRepository.UpdateAsync(tour);
    }
}
