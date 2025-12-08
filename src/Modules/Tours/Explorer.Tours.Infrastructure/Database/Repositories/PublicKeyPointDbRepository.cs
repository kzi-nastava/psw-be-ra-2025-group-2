using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class PublicKeyPointDbRepository : IPublicKeyPointRepository
{
    private readonly ToursContext _context;

    public PublicKeyPointDbRepository(ToursContext context)
    {
        _context = context;
    }

    public async Task<PublicKeyPoint?> GetByIdAsync(long id)
    {
        return await _context.PublicKeyPoints.FindAsync(id);
    }

    public async Task<IEnumerable<PublicKeyPoint>> GetByAuthorIdAsync(long authorId)
    {
        return await _context.PublicKeyPoints
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PublicKeyPoint>> GetApprovedAsync()
    {
        var approvedRequests = await _context.PublicKeyPointRequests
            .Include(r => r.PublicKeyPoint)
            .Where(r => r.Status == PublicKeyPointRequestStatus.Approved)
            .ToListAsync();

        return approvedRequests
            .Select(r => r.PublicKeyPoint)
            .Where(p => p != null)
            .OrderBy(p => p!.Name)
            .ToList()!;
    }

    public async Task AddAsync(PublicKeyPoint publicKeyPoint)
    {
        await _context.PublicKeyPoints.AddAsync(publicKeyPoint);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PublicKeyPoint publicKeyPoint)
    {
        _context.PublicKeyPoints.Update(publicKeyPoint);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(PublicKeyPoint publicKeyPoint)
    {
        _context.PublicKeyPoints.Remove(publicKeyPoint);
        await _context.SaveChangesAsync();
    }

    public async Task<PublicKeyPoint?> GetBySourceTourAndOrdinalNoAsync(long tourId, int ordinalNo)
    {
        return await _context.PublicKeyPoints
            .FirstOrDefaultAsync(p => p.SourceTourId == tourId && p.SourceOrdinalNo == ordinalNo);
    }


}