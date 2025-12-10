using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourReviewDbRepository : ITourReviewRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourReview> _dbSet;

    public TourReviewDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TourReview>();
    }

    public PagedResult<TourReview> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public TourReview Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new KeyNotFoundException("Not found: " + id);
        return entity;
    }

    public TourReview Create(TourReview entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public TourReview Update(TourReview entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new KeyNotFoundException(e.Message);
        }
        return entity;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public TourReview? GetByTouristAndTour(long touristId, long tourId)
    {
        return _dbSet.FirstOrDefault(r => r.TouristId == touristId && r.TourId == tourId);
    }

    public List<TourReview> GetAllByTourId(long tourId)
    {
        return _dbSet.Where(r => r.TourId == tourId).ToList();
    }

    public PagedResult<TourReview> GetByTourIdPaged(int page, int pageSize, long tourId)
    {
        var task = _dbSet
            .Where(r => r.TourId == tourId)
            .GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }
}
