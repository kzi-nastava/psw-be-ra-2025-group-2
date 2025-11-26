using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface ITouristPositionRepository
{
    TouristPosition Create(TouristPosition position);
    IEnumerable<TouristPosition> GetByPerson(long personId);
    TouristPosition? GetLatestByPerson(long personId);
}