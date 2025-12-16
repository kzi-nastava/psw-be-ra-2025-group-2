using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
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
        return await GetToursQueryWithIncludes()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tour>> GetByAuthorAsync(long authorId)
    {
        return await GetToursQueryWithKeyPoints()
            .Where(t => t.AuthorId == authorId)
            .ToListAsync();
    }

    public List<Tour> GetAllPublished(int page, int pageSize)
    {
        var query = GetPublishedToursQuery();

        if (ShouldReturnAllResults(page, pageSize))
            return query.ToList();

        return ApplyPagination(query, page, pageSize).ToList();
    }

    public async Task UpdateAsync(Tour tour)
    {
        try
        {
            UpdateEntityState(tour);
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

    public async Task<Tour?> GetTourWithKeyPointsAsync(long tourId)
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.Id == tourId);
    }

    public async Task<Tour?> GetTourByKeyPointIdAsync(long keyPointId)
    {
        return await GetToursQueryWithKeyPoints()
            .FirstOrDefaultAsync(t => t.KeyPoints.Any(kp => kp.Id == keyPointId));
    }

    public async Task<IEnumerable<Tour?>> GetAllAsync()
    {
        return await GetToursQueryWithKeyPoints().ToListAsync();
    }

    // Private helper methods - Query builders

    private IQueryable<Tour> GetToursQueryWithIncludes()
    {
        return _dbSet
            .Include(t => t.Equipment)
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations);
    }

    private IQueryable<Tour> GetToursQueryWithKeyPoints()
    {
        return _dbSet.Include(t => t.KeyPoints);
    }

    private IQueryable<Tour> GetPublishedToursQuery()
    {
        return GetToursQueryWithKeyPoints()
            .Where(t => t.Status == TourStatus.Published);
    }

    private static IQueryable<Tour> ApplyPagination(IQueryable<Tour> query, int page, int pageSize)
    {
        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    private static bool ShouldReturnAllResults(int page, int pageSize)
    {
        return page == 0 || pageSize == 0;
    }

    // Private helper methods - Update operations

    private void UpdateEntityState(Tour tour)
    {
        var entry = DbContext.Entry(tour);

        if (entry.State == EntityState.Detached)
        {
            DbContext.Update(tour);
        }
        else if (entry.State != EntityState.Modified)
        {
            entry.State = EntityState.Modified;
        }
    }


}