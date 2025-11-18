using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository clubRepository, IMapper mapper)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
        }

        public ClubDto Create(ClubDto club)
        {
            var entity = _mapper.Map<Club>(club);
            var created = _clubRepository.Create(entity);
            return _mapper.Map<ClubDto>(created);
        }

        public ClubDto Update(ClubDto club)
        {
            var entity = _mapper.Map<Club>(club);
            var updated = _clubRepository.Update(entity);
            return _mapper.Map<ClubDto>(updated);
        }

        public void Delete(long id)
        {
            _clubRepository.Delete(id);
        }

        public List<ClubDto> GetAll()
        {
            var clubs = _clubRepository.GetAll();
            return clubs.Select(_mapper.Map<ClubDto>).ToList();
        }
    }
}
