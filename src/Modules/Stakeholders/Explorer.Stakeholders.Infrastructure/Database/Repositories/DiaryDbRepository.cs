using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class DiaryDbRepository : IDiaryRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Diary> _dbSet;

    public DiaryDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Diary>();
    }

    public Diary Get(long id)
    {
        var entity = _dbSet
            .AsNoTracking()
            .FirstOrDefault(d => d.Id == id);

        if (entity == null) throw new NotFoundException("Diary not found: " + id);
        return entity;
    }

    public List<Diary> GetByUserId(long userId)
    {
        return _dbSet.Where(d => d.UserId == userId).ToList();
    }

    public Diary Create(Diary entity)
    {
        _dbSet.Add(entity);
        DbContext.SaveChanges();
        return entity;
    }

    public Diary Update(Diary entity)
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
            throw new NotFoundException("Diary not found: " + id);

        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public List<Diary> GetAll()
    {
        return _dbSet.ToList();
    }
}
