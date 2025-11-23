using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class ClubDbRepository : IClubRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Club> _dbSet;

    public ClubDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Club>();
    }

    public PagedResult<Club> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public Club Get(long id)
    {
        var entity = _dbSet
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == id);

        if (entity == null) throw new NotFoundException("Club not found: " + id);
        return entity;
    }

    public List<Club> GetByOwner(long ownerId)
    {
        return _dbSet.Where(c => c.OwnerId == ownerId).ToList();
    }

    public Club Create(Club entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Club Update(Club entity)
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
        var entity = _dbSet.Find(id);
        if (entity == null)
            throw new NotFoundException("Club not found: " + id);

        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    
    public List<Club> GetAll()
    {
        return _dbSet.ToList();
    }

}