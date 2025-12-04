using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;
public class MeetupDbRepository : IMeetupRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Meetup> _dbSet;

    public MeetupDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Meetup>();
    }

    public Meetup Create(Meetup meetup)
    {
        _dbSet.Add(meetup);
        DbContext.SaveChanges();
        return meetup;
    }

    public Meetup Update(Meetup meetup)
    {
        _dbSet.Update(meetup);
        DbContext.SaveChanges();
        return meetup;
    }

    public void Delete(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity == null)
        {
            return;
        }

        _dbSet.Remove(entity);
        DbContext.SaveChanges();
    }

    public IEnumerable<Meetup> GetAll()
    {
        return _dbSet.ToList();
    }

    public IEnumerable<Meetup> GetByCreator(long creatorId)
    {
        return _dbSet.Where(m => m.CreatorId == creatorId).ToList();
    }
}