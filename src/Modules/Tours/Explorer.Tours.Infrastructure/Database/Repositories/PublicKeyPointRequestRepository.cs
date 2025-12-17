using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class PublicKeyPointRequestRepository : IPublicKeyPointRequestRepository
{
    private readonly ToursContext _context;

    public PublicKeyPointRequestRepository(ToursContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PublicKeyPointRequest?> GetByIdAsync(long id)
    {
        return await GetRequestsQueryWithIncludes()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<PublicKeyPointRequest>> GetByAuthorIdAsync(long authorId)
    {
        return await GetRequestsQueryWithIncludes()
            .Where(r => r.AuthorId == authorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PublicKeyPointRequest>> GetPendingRequestsAsync()
    {
        return await GetPendingRequestsQuery()
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PublicKeyPointRequest request)
    {
        ValidateRequestNotNull(request);

        await _context.PublicKeyPointRequests.AddAsync(request);
        await _context.SaveChangesAsync();
        await LoadPublicKeyPointReference(request);
    }

    public async Task UpdateAsync(PublicKeyPointRequest request)
    {
        ValidateRequestNotNull(request);

        _context.PublicKeyPointRequests.Update(request);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsPendingRequestAsync(long keyPointId)
    {
        return await GetPendingRequestsQuery()
            .AnyAsync(r => r.PublicKeyPointId == keyPointId);
    }

    public async Task<Tour?> GetTourWithKeyPointAsync(long keyPointId)
    {
        return await _context.Tours
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.KeyPoints.Any(kp => kp.Id == keyPointId));
    }

    public async Task<PublicKeyPoint?> GetPublicKeyPointBySourceAsync(long tourId, int ordinalNo)
    {
        return await GetPublicKeyPointsQuery()
            .FirstOrDefaultAsync(pkp => pkp.SourceTourId == tourId && pkp.SourceOrdinalNo == ordinalNo);
    }

    public async Task AddPublicKeyPointAsync(PublicKeyPoint publicKeyPoint)
    {
        ValidatePublicKeyPointNotNull(publicKeyPoint);

        await _context.PublicKeyPoints.AddAsync(publicKeyPoint);
        await _context.SaveChangesAsync();
    }

    public async Task<PublicKeyPoint?> GetPublicKeyPointByIdAsync(long id)
    {
        return await GetPublicKeyPointsQuery()
            .FirstOrDefaultAsync(pkp => pkp.Id == id);
    }

    public async Task UpdatePublicKeyPointAsync(PublicKeyPoint publicKeyPoint)
    {
        ValidatePublicKeyPointNotNull(publicKeyPoint);

        _context.PublicKeyPoints.Update(publicKeyPoint);
        await _context.SaveChangesAsync();
    }
    public IEnumerable<PublicKeyPointRequest> GetPending()
    {
        return _context.PublicKeyPointRequests
            .Where(r => r.Status == PublicKeyPointRequestStatus.Pending)
            .ToList();
    }

    public PublicKeyPointRequest? Get(long id)
    {
        return _context.PublicKeyPointRequests.FirstOrDefault(r => r.Id == id);
    }

    public void Update(PublicKeyPointRequest request)
    {
        _context.PublicKeyPointRequests.Update(request);
        _context.SaveChanges();
    }
    private IQueryable<PublicKeyPointRequest> GetRequestsQueryWithIncludes()
    {
        return _context.PublicKeyPointRequests.Include(r => r.PublicKeyPoint);
    }

    private IQueryable<PublicKeyPointRequest> GetPendingRequestsQuery()
    {
        return GetRequestsQueryWithIncludes()
            .Where(r => r.Status == PublicKeyPointRequestStatus.Pending);
    }

    private IQueryable<PublicKeyPoint> GetPublicKeyPointsQuery()
    {
        return _context.PublicKeyPoints;
    }

    private async Task LoadPublicKeyPointReference(PublicKeyPointRequest request)
    {
        await _context.Entry(request)
            .Reference(r => r.PublicKeyPoint)
            .LoadAsync();
    }

    private static void ValidateRequestNotNull(PublicKeyPointRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
    }

    private static void ValidatePublicKeyPointNotNull(PublicKeyPoint publicKeyPoint)
    {
        if (publicKeyPoint == null)
            throw new ArgumentNullException(nameof(publicKeyPoint));
    }

    public async Task<IEnumerable<PublicKeyPointRequest>> GetBySourceAsync(long tourId, int ordinalNo)
    {
        return await _context.PublicKeyPointRequests 
            .Include(r => r.PublicKeyPoint)
            .Where(r => r.PublicKeyPoint.SourceTourId == tourId &&
                        r.PublicKeyPoint.SourceOrdinalNo == ordinalNo)
            .ToListAsync();
    }

    public async Task DeleteAsync(PublicKeyPointRequest request)
    {
        _context.PublicKeyPointRequests.Remove(request);  
        await _context.SaveChangesAsync();
    }
}