using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class UserDbRepository : IUserRepository
{
    private readonly StakeholdersContext _dbContext;
    private readonly DbSet<User> _dbSet;

    public UserDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<User>();
    }

    public bool Exists(string username)
    {
        return _dbContext.Users.Any(user => user.Username == username);
    }

    public User? GetActiveByName(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username && user.IsActive);
    }

    public User? GetActiveById(long id)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Id == id && user.IsActive);
    }
    public User Create(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return user;
    }

    public long GetPersonId(long userId)
    {
        var person = _dbContext.People.FirstOrDefault(i => i.UserId == userId);
        if (person == null) throw new KeyNotFoundException("Not found.");
        return person.Id;
    }
    public User? GetUserByUsername(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username);
    }
    public User Update(User user)
    {
        try
        {
            _dbContext.Update(user);
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new NotFoundException(e.Message);
        }
        return user;
    }
    public PagedResult<User> GetPaged(int page, int pageSize)
    {
        var task = _dbSet.GetPagedById(page, pageSize);
        task.Wait();
        return task.Result;
    }

    public User GetByPersonId(long personId)
    {
        return _dbContext.Users.FirstOrDefault(p => p.Id == personId);
    }
    public User? Get(long id)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Id == id);
    }

    public List<User> GetTourists(string? query)
    {
        var usersQuery = _dbContext.Users
            .Where(u => u.Role == UserRole.Tourist)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.Trim().ToLowerInvariant();
            usersQuery = usersQuery.Where(u => u.Username.ToLower().Contains(q));
        }

        return usersQuery
            .OrderBy(u => u.Username)
            .ToList();
    }

    public List<User> GetAllActiveUsers()
    {
        return _dbContext.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToList();
    }
}