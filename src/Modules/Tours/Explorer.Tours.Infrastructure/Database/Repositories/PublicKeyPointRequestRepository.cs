using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class PublicKeyPointRequestRepository : IPublicKeyPointRequestRepository
{
    private readonly ToursContext _context;

    public PublicKeyPointRequestRepository(ToursContext context)
    {
        _context = context;
    }

    public async Task<PublicKeyPointRequest?> GetByIdAsync(long id)
    {
        return await _context.PublicKeyPointRequests
            .Include(r => r.PublicKeyPoint)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<PublicKeyPointRequest>> GetByAuthorIdAsync(long authorId)
    {
        return await _context.PublicKeyPointRequests
            .Include(r => r.PublicKeyPoint) 
            .Where(r => r.AuthorId == authorId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PublicKeyPointRequest>> GetPendingRequestsAsync()
    {
        return await _context.PublicKeyPointRequests
            .Include(r => r.PublicKeyPoint)
            .Where(r => r.Status == PublicKeyPointRequestStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(PublicKeyPointRequest request)
    {
        await _context.PublicKeyPointRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PublicKeyPointRequest request)
    {
        _context.PublicKeyPointRequests.Update(request);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsPendingRequestAsync(long keyPointId)
    {
        return await _context.PublicKeyPointRequests
            .AnyAsync(r => r.PublicKeyPointId == keyPointId && r.Status == PublicKeyPointRequestStatus.Pending);
    }
    public async Task<Tour?> GetTourWithKeyPointAsync(long keyPointId)
    {
        return await _context.Tours
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.KeyPoints.Any(kp => kp.Id == keyPointId));
    }

    public async Task<PublicKeyPoint?> GetPublicKeyPointBySourceAsync(long tourId, int ordinalNo)
    {
        return await _context.PublicKeyPoints
            .FirstOrDefaultAsync(pkp => pkp.SourceTourId == tourId && pkp.SourceOrdinalNo == ordinalNo);
    }

    public async Task AddPublicKeyPointAsync(PublicKeyPoint publicKeyPoint)
    {
        await _context.PublicKeyPoints.AddAsync(publicKeyPoint);
        await _context.SaveChangesAsync();
    }
    public async Task<PublicKeyPoint?> GetPublicKeyPointByIdAsync(long id)
    {
        return await _context.PublicKeyPoints
            .FirstOrDefaultAsync(pkp => pkp.Id == id);
    }

    public async Task UpdatePublicKeyPointAsync(PublicKeyPoint publicKeyPoint)
    {
        _context.PublicKeyPoints.Update(publicKeyPoint);
        await _context.SaveChangesAsync();
    }
}