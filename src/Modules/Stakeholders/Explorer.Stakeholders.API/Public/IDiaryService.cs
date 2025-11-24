using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IDiaryService
{
    DiaryDto Create(DiaryDto diary);
    DiaryDto Update(DiaryDto diary);
    void Delete(long id);
    List<DiaryDto> GetByUserId(long userId);
    DiaryDto Get(long id);
}