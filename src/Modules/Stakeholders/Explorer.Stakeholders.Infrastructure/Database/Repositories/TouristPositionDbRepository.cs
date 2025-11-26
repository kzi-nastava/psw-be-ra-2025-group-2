using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class TouristPositionDbRepository : ITouristPositionRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<TouristPosition> _dbSet;

    public TouristPositionDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<TouristPosition>();
    }

    public TouristPosition Create(TouristPosition position)
    {
        _dbSet.Add(position);
        DbContext.SaveChanges();
        return position;
    }

    public IEnumerable<TouristPosition> GetByPerson(long personId)
    {
        return _dbSet.Where(p => p.PersonId == personId).OrderBy(p => p.RecordedAt).ToList();
    }

    public TouristPosition? GetLatestByPerson(long personId)
    {
        return _dbSet.Where(p => p.PersonId == personId).OrderByDescending(p => p.RecordedAt).FirstOrDefault();
    }
}