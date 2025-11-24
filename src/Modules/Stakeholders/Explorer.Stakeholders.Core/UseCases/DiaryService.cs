using AutoMapper;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;

public class DiaryService : IDiaryService
{
    private readonly IDiaryRepository _diaryRepository;
    private readonly IMapper _mapper;

    public DiaryService(IDiaryRepository diaryRepository, IMapper mapper)
    {
        _diaryRepository = diaryRepository;
        _mapper = mapper;
    }

    public DiaryDto Create(DiaryDto diary)
    {
        var entity = _mapper.Map<Diary>(diary);
        var result = _diaryRepository.Create(entity);
        return _mapper.Map<DiaryDto>(result);
    }

    public DiaryDto Update(DiaryDto diary)
    {
        var entity = _mapper.Map<Diary>(diary);
        var result = _diaryRepository.Update(entity);
        return _mapper.Map<DiaryDto>(result);
    }

    public void Delete(long id)
    {
        _diaryRepository.Delete(id);
    }

    public List<DiaryDto> GetByUserId(long userId)
    {
        var diaries = _diaryRepository.GetByUserId(userId);
        return diaries.Select(_mapper.Map<DiaryDto>).ToList();
    }

    public DiaryDto Get(long id)
    {
        var diary = _diaryRepository.Get(id);
        return _mapper.Map<DiaryDto>(diary);
    }
}
