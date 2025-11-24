namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPersonRepository
{
    Person Create(Person person);
    Person GetByUserId(long userId);
    Person Update(Person person);
}