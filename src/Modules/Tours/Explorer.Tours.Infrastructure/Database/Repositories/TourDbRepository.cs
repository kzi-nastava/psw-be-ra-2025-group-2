using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourDbRepository : ITourRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<Tour> _dbSet;

    public TourDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Tour>();
    }

    public async Task<Tour> AddAsync(Tour tour)
    {
        await _dbSet.AddAsync(tour);
        await DbContext.SaveChangesAsync();
        return tour;
    }

    public async Task<Tour?> GetByIdAsync(long id)
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tour>> GetByAuthorAsync(long authorId)
    {
        return await _dbSet
             .Include(t => t.KeyPoints)
            .Where(t => t.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Tour tour)
    {
        try
        {
            DbContext.Update(tour);
            await DbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
    }

    public async Task DeleteAsync(Tour tour)
    {
        _dbSet.Remove(tour);
        await DbContext.SaveChangesAsync();
    }
}
