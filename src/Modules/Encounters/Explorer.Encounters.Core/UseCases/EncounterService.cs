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
        private readonly IEncounterExecutionRepository _executionRepository;
        private readonly IMapper _mapper;
        private readonly IEncounterPresenceRepository _presenceRepository;

        public EncounterService(IEncounterRepository repository, IEncounterExecutionRepository executionRepository, IMapper mapper, IEncounterPresenceRepository presenceRepository)
        {
            _encounterRepository = repository;
            _executionRepository = executionRepository;
            _mapper = mapper;
            _presenceRepository = presenceRepository;
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
            EncounterType type;
            if (!Enum.TryParse(createDto.Type, true, out type))
            {
                throw new ArgumentException("Invalid encounter type");
            }

            Encounter encounter;

            switch (type)
            {
                case EncounterType.Social:
                    encounter = new SocialEncounter(
                        createDto.Name,
                        createDto.Description,
                        new GeoLocation(createDto.Latitude, createDto.Longitude),
                        new ExperiencePoints(createDto.XP),
                        createDto.RequiredPeople ?? 5,
                        createDto.Range ?? 10
                    );
                    break;

                case EncounterType.Location:
                    encounter = new HiddenLocationEncounter(
                        createDto.Name,
                        createDto.Description,
                        new GeoLocation(createDto.Latitude, createDto.Longitude),
                        new ExperiencePoints(createDto.XP),
                        createDto.ImageUrl ?? string.Empty,
                        new GeoLocation(createDto.ImageLatitude ?? 0, createDto.ImageLongitude ?? 0),
                        createDto.DistanceTreshold ?? 5
                    );
                    break;

                case EncounterType.Miscellaneous:
                default:
                    encounter = new MiscEncounter(
                        createDto.Name,
                        createDto.Description,
                        new GeoLocation(createDto.Latitude, createDto.Longitude),
                        new ExperiencePoints(createDto.XP)
                    );
                    break;
            }

            var created = _encounterRepository.Create(encounter);
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

        public void CompleteEncounter(long userId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
                throw new NotFoundException("Encounter not found");

            if (encounter.Type != EncounterType.Miscellaneous)
                throw new InvalidOperationException("Only Miscellaneous encounters can be completed manually.");

            bool alreadyCompleted = _executionRepository.IsCompleted(userId, encounterId);

            var execution = new EncounterExecution(userId, encounterId);
            _executionRepository.Add(execution);

            // Logika za XP
            if (!alreadyCompleted)
            {
                // Korisnik prvi put rešava izazov - Daj mu XP
                // TODO: Pozovi servis za dodavanje XP-a korisniku
            }
            else
            {
                // Korisnik je već rešio ovo ranije. 
                // Ovde ne radimo ništa oko XP-a
            }
        }

        public SocialPresenceStatusDto PingSocialPresence(long userId, long encounterId, SocialPresencePingDto ping)
        {
            var encounter = _encounterRepository.GetById(encounterId)
                ?? throw new NotFoundException("Encounter not found");

            if (encounter.Type != EncounterType.Social)
                throw new InvalidOperationException("Encounter is not social.");

            if (!encounter.IsActive())
                throw new InvalidOperationException("Encounter is not active.");

            var social = (SocialEncounter)encounter;

            const int PresenceTtlSeconds = 25; // ping na 10s + tolerancija
            var now = DateTime.UtcNow;
            var cutoff = now.AddSeconds(-PresenceTtlSeconds);

            var distance = GeoDistance.DistanceMeters(
                ping.Latitude, ping.Longitude,
                encounter.Location.Latitude, encounter.Location.Longitude
            );

            bool inRange = distance <= social.Range;

            // 1) Očisti stare prisustva
            _presenceRepository.RemoveOlderThan(encounterId, cutoff);

            // 2) Upsert ili remove za ovog korisnika
            if (inRange)
                _presenceRepository.Upsert(encounterId, userId, ping.Latitude, ping.Longitude);
            else
                _presenceRepository.Remove(encounterId, userId);

            // 3) Prebroj aktivne u opsegu
            var activeUserIds = _presenceRepository.GetActiveUserIds(encounterId, cutoff);
            var activeCount = activeUserIds.Count;

            // 4) Kad ih ima dovoljno, reši za svakog aktivnog koji još nije rešio
            bool justCompleted = false;
            if (activeCount >= social.RequiredPeople)
            {
                foreach (var uid in activeUserIds)
                {
                    if (_executionRepository.IsCompleted(uid, encounterId)) continue;

                    _executionRepository.Add(new EncounterExecution(uid, encounterId));
                    justCompleted = true;
                }
            }

            return new SocialPresenceStatusDto
            {
                InRange = inRange,
                ActiveCount = activeCount,
                RequiredPeople = social.RequiredPeople,
                JustCompleted = justCompleted
            };
        }

    }
}
