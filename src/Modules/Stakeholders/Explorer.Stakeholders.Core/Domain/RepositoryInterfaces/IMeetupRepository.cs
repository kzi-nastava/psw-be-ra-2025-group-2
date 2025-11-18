using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IMeetupRepository
{
    Meetup Create(Meetup meetup);
    Meetup Update(Meetup meetup);
    void Delete(long id);
    IEnumerable<Meetup> GetAll();
    IEnumerable<Meetup> GetByCreator(long creatorId);
}