using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class TouristObjectService : ITouristObjectService
{
    private readonly ITouristObjectRepository _touristObjectRepository;
    private readonly IMapper _mapper;

    public TouristObjectService(ITouristObjectRepository repository, IMapper mapper)
    {
        _touristObjectRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<TouristObjectDto> GetPaged(int page, int pageSize)
    {
        var result = _touristObjectRepository.GetPaged(page, pageSize);

        var items = result.Results.Select(_mapper.Map<TouristObjectDto>).ToList();
        return new PagedResult<TouristObjectDto>(items, result.TotalCount);
    }

    public TouristObjectDto Create(TouristObjectDto entity)
    {
        var result = _touristObjectRepository.Create(_mapper.Map<TouristObject>(entity));
        return _mapper.Map<TouristObjectDto>(result);
    }

    public TouristObjectDto Update(TouristObjectDto entity)
    {
        var result = _touristObjectRepository.Update(_mapper.Map<TouristObject>(entity));
        return _mapper.Map<TouristObjectDto>(result);
    }

    public void Delete(long id)
    {
        _touristObjectRepository.Delete(id);
    }
}
