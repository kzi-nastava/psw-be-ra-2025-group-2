using System.Collections.Generic;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourProblemDbRepository : ITourProblemRepository
{
    protected readonly ToursContext DbContext;
    private readonly DbSet<TourProblem> _dbSet;

    public TourProblemDbRepository(ToursContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TourProblem>();
    }

    public TourProblem Get(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null) throw new NotFoundException("Not found: " + id);
        return entity;
    }

    public TourProblem Create(TourProblem entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public List<TourProblem> GetForCreator()
    {
        return _dbSet.ToList();
    }

    public TourProblem Update(TourProblem entity)
    {
        try
        {
            DbContext.Update(entity);
            DbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }

        return entity;
    }

    public void Delete(long id)
    {
        var entity = Get(id);
        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }
}
