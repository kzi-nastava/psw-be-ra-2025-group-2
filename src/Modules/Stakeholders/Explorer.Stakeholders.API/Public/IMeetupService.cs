using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IMeetupService
{
    MeetupDto Create(MeetupDto dto);
    MeetupDto Update(MeetupDto dto);
    void Delete(long id);
    IEnumerable<MeetupDto> GetAll();
    IEnumerable<MeetupDto> GetByCreator(long creatorId);
}