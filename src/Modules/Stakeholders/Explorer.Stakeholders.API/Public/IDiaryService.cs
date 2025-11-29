using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IDiaryService
{
    DiaryDto Create(DiaryDto diary);
    DiaryDto Update(DiaryDto diary);
    void Delete(long id);
    List<DiaryDto> GetAll();
    DiaryDto Get(long id);
}