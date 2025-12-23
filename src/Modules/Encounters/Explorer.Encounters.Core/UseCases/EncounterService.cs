using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterService : IEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IMapper _mapper;

        public EncounterService(IEncounterRepository repository, IMapper mapper)
        {
            _encounterRepository = repository;
            _mapper = mapper;
        }

        public void Archive(long id)
        {
            var encounter = _encounterRepository.GetById(id);

            if(encounter == null)
            {
                throw new NotFoundException($"Not found: {id}");
            }

            encounter.Archive();
            _encounterRepository.Update(encounter);
        }

        public EncounterDto Create(CreateEncounterDto createDto)
        {
            var newEncounter = _mapper.Map<Encounter>(createDto);

            var created = _encounterRepository.Create(newEncounter);

            return _mapper.Map<EncounterDto>(created);
        }

        public void Delete(long id)
        {
            _encounterRepository.Delete(id);
        }

        public EncounterDto Get(long id)
        {
            var encounter = _encounterRepository.GetById(id);

            if (encounter == null)
                throw new NotFoundException($"Not found: {id}");

            return _mapper.Map<EncounterDto>(encounter);
        }

        public IEnumerable<EncounterDto> GetActive()
        {
            var active = _encounterRepository.GetActive();
            return _mapper.Map<IEnumerable<EncounterDto>>(active);
        }

        public int GetCount()
        {
            return _encounterRepository.GetCount();
        }

        public PagedResult<EncounterDto> GetPaged(int page, int pageSize)
        {
            var paged = _encounterRepository.GetPaged(page, pageSize);

            var dtos = paged.Results.Select(_mapper.Map<EncounterDto>).ToList();

            return new PagedResult<EncounterDto>(dtos, dtos.Count);
        }

        public void MakeActive(long id)
        {
            var encounter = _encounterRepository.GetById(id);

            if (encounter == null)
                throw new NotFoundException($"Not found: {id}");

            encounter.MakeActive();
            _encounterRepository.Update(encounter);
        }

        public EncounterDto Update(UpdateEncounterDto updateDto)
        {
            var encounter = _encounterRepository.GetById(updateDto.Id);

            if (encounter == null)
                throw new NotFoundException($"Not found: {updateDto.Id}");

            encounter.Update(updateDto.Name, updateDto.Description,
                new GeoLocation(updateDto.Latitude, updateDto.Longitude), new ExperiencePoints(updateDto.XP), EncounterTypeParser.Parse(updateDto.Type));

            var updated = _encounterRepository.Update(encounter);

            return _mapper.Map<EncounterDto>(updated);
        }
    }
}
