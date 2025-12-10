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
        try
        {
            // 1. Pronađi Tour sa KeyPoint-ovima
            var tour = await _tourRepository.GetTourWithKeyPointsAsync(tourId);
            if (tour == null)
                throw new KeyNotFoundException("Tour not found.");

            // 2. Pronađi KeyPoint po ordinalnom broju
            var keyPoint = tour.KeyPoints.FirstOrDefault(kp => kp.OrdinalNo == ordinalNo);
            if (keyPoint == null)
                throw new KeyNotFoundException("KeyPoint not found.");

            // ✅ DODAJ DEBUG INFO
            Console.WriteLine($"KeyPoint found: Name={keyPoint.Name}, Lat={keyPoint.Latitude}, Lon={keyPoint.Longitude}");

            // 3. Proveri da li već postoji PublicKeyPoint
            var existingPublicKeyPoint = await _requestRepository
                .GetPublicKeyPointBySourceAsync(tourId, ordinalNo);

            PublicKeyPoint publicKeyPoint;

            if (existingPublicKeyPoint != null)
            {
                publicKeyPoint = existingPublicKeyPoint;
            }
            else
            {
                // 4. Kreiraj novi PublicKeyPoint
                publicKeyPoint = PublicKeyPoint.CreateFromKeyPoint(keyPoint, authorId, tourId);

                // ✅ DODAJ DEBUG INFO
                Console.WriteLine($"Creating PublicKeyPoint: Lat={publicKeyPoint.Latitude}, Lon={publicKeyPoint.Longitude}");

                await _requestRepository.AddPublicKeyPointAsync(publicKeyPoint);
            }

            // 5. Proveri pending request
            if (await _requestRepository.ExistsPendingRequestAsync(publicKeyPoint.Id))
                throw new InvalidOperationException("A request for this item has already been sent.");

            // 6. Kreiraj request
            var request = new PublicKeyPointRequest(publicKeyPoint.Id, authorId);
            await _requestRepository.AddAsync(request);

            // ✅ Učitaj ponovo sa svim podacima i navigation properties
            var savedRequest = await _requestRepository.GetByIdAsync(request.Id);

            return _mapper.Map<PublicKeyPointRequestDto>(savedRequest);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
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
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new KeyNotFoundException("Request not found.");

        // 1. Odobri zahtev
        request.Approve(adminId);
        await _requestRepository.UpdateAsync(request);

        // 2. ✅ AŽURIRAJ STATUS KEYPONTA NA APPROVED
        var publicKeyPoint = await _requestRepository.GetPublicKeyPointByIdAsync(request.PublicKeyPointId);

        if (publicKeyPoint != null)
        {
            publicKeyPoint.Approve(); // ⬅️ Pozovi postojeću metodu
            await _requestRepository.UpdatePublicKeyPointAsync(publicKeyPoint);
        }

        // 3. Notifikuj autora
        await _notificationService.NotifyAuthorApprovedAsync(request.AuthorId, "Keypoint");

        return _mapper.Map<PublicKeyPointRequestDto>(request);
    }

    public async Task<PublicKeyPointRequestDto> RejectRequestAsync(long requestId, long adminId, string? reason)
    {
        var request = await _requestRepository.GetByIdAsync(requestId)
            ?? throw new KeyNotFoundException("Request not found.");

        request.Reject(adminId, reason);
        await _requestRepository.UpdateAsync(request);

        await _notificationService.NotifyAuthorRejectedAsync(request.AuthorId, "Keypoint", reason);

        return _mapper.Map<PublicKeyPointRequestDto>(request);
    }

    public async Task WithdrawRequestAsync(long keyPointId, long authorId)
    {
        throw new NotImplementedException();
    }
}