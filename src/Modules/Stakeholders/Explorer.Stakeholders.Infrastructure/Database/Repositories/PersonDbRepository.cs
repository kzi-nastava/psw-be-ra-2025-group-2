using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PersonDbRepository : IPersonRepository
{
    protected readonly StakeholdersContext DbContext;
    private readonly DbSet<Person> _dbSet;

    public PersonDbRepository(StakeholdersContext dbContext)
    {
        DbContext = dbContext;
        _dbSet = DbContext.Set<Person>();
    }

    public Person Create(Person person)
    {
        _dbSet.Add(person);
        DbContext.SaveChanges();
        return person;
    }
    public Person GetByUserId(long userId)
    {
        return _dbSet.FirstOrDefault(x => x.UserId == userId);
    }

    public Person Update(Person person)
    {
        _dbSet.Update(person);
        DbContext.SaveChanges();
        return person;
    }
}