using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;
public class MeetupService : IMeetupService
{
    private readonly IMeetupRepository _repo;
    private readonly IMapper _mapper;

    public MeetupService(IMeetupRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public MeetupDto Create(MeetupDto dto)
    {
        var result = _repo.Create(_mapper.Map<Meetup>(dto));
        return _mapper.Map<MeetupDto>(result);
    }

    public MeetupDto Update(MeetupDto dto)
    {
        var result = _repo.Update(_mapper.Map<Meetup>(dto));
        return _mapper.Map<MeetupDto>(result);
    }

    public void Delete(long id)
    {
        _repo.Delete(id);
    }

    public IEnumerable<MeetupDto> GetAll()
    {
        return _repo.GetAll()
            .Select(_mapper.Map<MeetupDto>)
            .ToList();
    }

    public IEnumerable<MeetupDto> GetByCreator(long creatorId)
    {
        return _repo.GetByCreator(creatorId)
            .Select(_mapper.Map<MeetupDto>)
            .ToList();
    }
}