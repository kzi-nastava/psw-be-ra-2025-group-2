using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        ClubDto Create(ClubDto club);
        ClubDto Update(ClubDto club);
        void Delete(long id);
        List<ClubDto> GetAll();
        ClubDto Get(long id);
    }
}
