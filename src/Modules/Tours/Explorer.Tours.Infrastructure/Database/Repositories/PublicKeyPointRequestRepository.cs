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

    public async Task<IEnumerable<PublicKeyPointRequest>> GetPendingAsync()
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

    public async Task<bool> ExistsPendingForKeyPointAsync(long publicKeyPointId)
    {
        return await _context.PublicKeyPointRequests
            .AnyAsync(r => r.PublicKeyPointId == publicKeyPointId &&
                          r.Status == PublicKeyPointRequestStatus.Pending);
    }

    public async Task<PublicKeyPointRequest?> GetLatestByPublicKeyPointIdAsync(long publicKeyPointId)
    {
        return await _context.PublicKeyPointRequests
            .Where(r => r.PublicKeyPointId == publicKeyPointId)
            .OrderByDescending(r => r.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<PublicKeyPointRequest>> GetByPublicKeyPointIdsAsync(IEnumerable<long> publicKeyPointIds)
    {
        return await _context.PublicKeyPointRequests
            .Where(r => publicKeyPointIds.Contains(r.PublicKeyPointId))
            .ToListAsync();
    }
}