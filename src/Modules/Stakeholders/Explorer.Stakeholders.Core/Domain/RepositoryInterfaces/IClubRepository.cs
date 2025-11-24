using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubRepository
    {
        Club Create(Club club);
        Club Get(long id);
        Club Update(Club club);
        void Delete(long id);
        List<Club> GetAll();
    }
}
