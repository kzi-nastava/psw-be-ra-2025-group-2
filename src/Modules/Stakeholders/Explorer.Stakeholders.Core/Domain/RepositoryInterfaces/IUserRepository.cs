using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    bool Exists(string username);
    User? GetActiveByName(string username);
    User Create(User user);
    long GetPersonId(long userId);
    User? GetUserByUsername(string username);
    User Update(User user);
    PagedResult<User> GetPaged(int page, int pageSize);
    User GetByPersonId(long personId);
    User? Get(long id);
    List<User> GetTourists(string? query);
    List<User> GetAllActiveUsers();
}