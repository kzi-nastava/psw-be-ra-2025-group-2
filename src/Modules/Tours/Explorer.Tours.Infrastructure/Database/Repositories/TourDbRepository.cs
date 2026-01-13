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
        return await _dbSet
            .Include(t => t.Equipment)
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public IEnumerable<Tour> GetByIds(IEnumerable<long> ids)
    {
        return _dbSet.Where(t => ids.Contains(t.Id)).ToList();
    }
    public async Task<IEnumerable<Tour>> GetByAuthorAsync(long authorId)
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .Include(t => t.Reviews)
            .Where(t => t.AuthorId == authorId)
            .ToListAsync();
    }

    public List<Tour> GetAllPublished(int page, int pageSize)
    {
        var query = _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .Include(t => t.Reviews)
            .Where(t => t.Status == TourStatus.Published);

        if (page <= 0 || pageSize <= 0)
            return query.ToList();

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public List<Tour> GetAllPublished()
    {
        return _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .Include(t => t.Reviews)
            .Where(t => t.Status == TourStatus.Published)
            .ToList();
    }

    public List<Tour> GetAllNonDrafts()
    {
        return _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .Include(t => t.Reviews)
            .Where(t => t.Status != TourStatus.Draft)
            .ToList();
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


    public List<Tour> GetByIds(List<long> ids)
    {
        return _dbSet
            .Where(t => ids.Contains(t.Id))
            .ToList();

    }

    public async Task<Tour?> GetTourWithKeyPointsAsync(long tourId)
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .Include(t => t.Reviews)
            .FirstOrDefaultAsync(t => t.Id == tourId);
    }

    public async Task<Tour?> GetTourByKeyPointIdAsync(long keyPointId)
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .FirstOrDefaultAsync(t => t.KeyPoints.Any(kp => kp.Id == keyPointId));
    }

    public async Task<IEnumerable<Tour?>> GetAllAsync()
    {
        return await _dbSet
            .Include(t => t.KeyPoints)
            .Include(t => t.Durations)
            .Include(t => t.Reviews)
            .ToListAsync();
    }

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