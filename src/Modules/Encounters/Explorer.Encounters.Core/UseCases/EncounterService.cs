using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos.Encounter;
using Explorer.Encounters.API.Dtos.TouristProgress;
using Explorer.Encounters.API.Dtos.EncounterExecution;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.API.Internal;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterService : IEncounterService, IInternalEncounterExecutionService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IEncounterExecutionRepository _executionRepository;
        private readonly ITouristProgressRepository _touristProgressRepository;
        private readonly IMapper _mapper;
        private readonly IEncounterPresenceRepository _presenceRepository;


        private const int HiddenRequiredSecondsInZone = 30;
        private const int DefaultPingDeltaSeconds = 10;

        public EncounterService(
        IEncounterRepository repository,
        IEncounterExecutionRepository executionRepository,
        ITouristProgressRepository touristProgressRepository,
        IEncounterPresenceRepository presenceRepository, 
        IMapper mapper)
        {
            _encounterRepository = repository;
            _executionRepository = executionRepository;
            _touristProgressRepository = touristProgressRepository;
            _mapper = mapper;
            _presenceRepository = presenceRepository;
        }

        public void Archive(long id)
        {
            var encounter = _encounterRepository.GetById(id);

            if (encounter == null)
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

            encounter.Update(
                updateDto.Name,
                updateDto.Description,
                new GeoLocation(updateDto.Latitude, updateDto.Longitude),
                new ExperiencePoints(updateDto.XP),
                EncounterTypeParser.Parse(updateDto.Type)
            );


            if (encounter.Type == EncounterType.Location)
            {
                var hidden = (HiddenLocationEncounter)encounter;


                hidden.UpdateHiddenLocation(
                    updateDto.ImageUrl ?? hidden.ImageUrl,
                    new GeoLocation(updateDto.ImageLatitude ?? hidden.ImageLocation.Latitude,
                        updateDto.ImageLongitude ?? hidden.ImageLocation.Longitude),
                    updateDto.DistanceTreshold ?? hidden.DistanceTreshold
                );
            }

            var updated = _encounterRepository.Update(encounter);

            return _mapper.Map<EncounterDto>(updated);
        }
        /// <summary>
        /// Creates an "in-progress" execution if not already completed and if none exists.
        /// </summary>
        public void ActivateEncounter(
            long userId,
            long encounterId,
            double latitude,
            double longitude)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
                throw new NotFoundException("Encounter not found.");

            if (!encounter.IsActive())
                throw new InvalidOperationException("Encounter is not active.");


            if (_executionRepository.IsCompleted(userId, encounterId))
                return;


            var existing = _executionRepository.Get(userId, encounterId);
            if (existing != null && !existing.IsCompleted)
                return;


            if (encounter.Type == EncounterType.Location)
            {
                var hidden = (HiddenLocationEncounter)encounter;

                var distanceToEncounter = CalculateDistanceMeters(
                    latitude,
                    longitude,
                    hidden.Location.Latitude,
                    hidden.Location.Longitude
                );


                if (distanceToEncounter > hidden.DistanceTreshold)
                {
                    throw new InvalidOperationException(
                        $"You cannot activate this challenge until you are within {hidden.DistanceTreshold:0}m of the location."
                    );
                }
            }


            _executionRepository.Add(
                new EncounterExecution(userId, encounterId)
            );
        }


        /// <summary>
        /// Called every ~10 seconds by the tourist app. Updates execution progress.
        /// For HiddenLocation: must accumulate 30 seconds inside DistanceTreshold to complete.
        /// </summary>
        public (bool IsCompleted, int SecondsInsideZone, int RequiredSeconds, DateTime? CompletionTime) PingLocation(
            long userId,
            long encounterId,
            double latitude,
            double longitude,
            int? deltaSeconds = null)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
                throw new NotFoundException("Encounter not found");

            if (encounter.Type != EncounterType.Location)
                throw new InvalidOperationException("Location ping is only supported for Hidden Location encounters.");

            if (!encounter.IsActive())
                throw new InvalidOperationException("Encounter is not active.");

            var hidden = encounter as HiddenLocationEncounter;
            if (hidden == null)
                throw new InvalidOperationException("Encounter is not a HiddenLocationEncounter.");

            // Ensure execution exists
            var execution = _executionRepository.Get(userId, encounterId);
            if (execution == null)
                throw new InvalidOperationException("Encounter nije aktiviran.");

            if (execution.IsCompleted)
                return (execution.IsCompleted, execution.SecondsInsideZone, HiddenRequiredSecondsInZone, execution.CompletionTime);

            var ds = deltaSeconds ?? DefaultPingDeltaSeconds;

            var distanceMeters = CalculateDistanceMeters(
                latitude, longitude,
                hidden.ImageLocation.Latitude, hidden.ImageLocation.Longitude);


            var isInside = distanceMeters <= hidden.DistanceTreshold;

            execution.RegisterPing(isInside, ds);

            if (!execution.IsCompleted && execution.SecondsInsideZone >= HiddenRequiredSecondsInZone)
            {
                // markira completed i upise XP u execution
                execution.MarkCompleted(hidden.XP.Value);

                // doda xp i level up ako treba
                var progress = _touristProgressRepository.GetByUserId(userId)
                              ?? _touristProgressRepository.Create(new TouristProgress(userId));

                progress.AddXp(hidden.XP.Value);
                _touristProgressRepository.Update(progress);
            }

            _executionRepository.Update(execution);

            return (execution.IsCompleted, execution.SecondsInsideZone, HiddenRequiredSecondsInZone, execution.CompletionTime);
        }
        private static double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters

            static double ToRad(double deg) => deg * Math.PI / 180.0;

            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        public void CompleteEncounter(long userId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId)
                ?? throw new NotFoundException("Encounter not found");


            if (_executionRepository.IsCompleted(userId, encounterId))
                return;

            var execution = _executionRepository.Get(userId, encounterId);

            if (execution == null)
            {
                execution = new EncounterExecution(userId, encounterId);
                
                execution.MarkCompleted(encounter.XP.Value);

                _executionRepository.Add(execution);
            }
            else
            {

                execution.MarkCompleted(encounter.XP.Value);
                _executionRepository.Update(execution);
            }

            var progress = _touristProgressRepository.GetByUserId(userId)
                          ?? _touristProgressRepository.Create(new TouristProgress(userId));

            progress.AddXp(encounter.XP.Value);
            _touristProgressRepository.Update(progress);
        }

        public EncounterDto CreateByTourist(long userId, CreateEncounterDto createDto)
        {
            // ako ne postoji nikakav progres kreira ga na level 1
            var progress = _touristProgressRepository.GetByUserId(userId)
                          ?? _touristProgressRepository.Create(new TouristProgress(userId));

            // provera nivoa
            if (progress.Level < 10)
                throw new UnauthorizedAccessException("You must reach level 10 to create challenges.");

            // kreiranje encountera
            return Create(createDto);
        }
        public TouristProgressDto GetMyProgress(long userId)
        {
            var progress = _touristProgressRepository.GetByUserId(userId)
                          ?? _touristProgressRepository.Create(new TouristProgress(userId));

            return new TouristProgressDto
            {
                UserId = progress.UserId,
                TotalXp = progress.TotalXp,
                Level = progress.Level,
                CanCreateChallenges = progress.CanCreateChallenges
            };
        }


        public (bool IsCompleted, int SecondsInsideZone, int RequiredSeconds, DateTime? CompletionTime) GetExecutionStatus(long userId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
                throw new NotFoundException("Encounter not found");

            var exec = _executionRepository.Get(userId, encounterId);

            if (exec == null)
                throw new InvalidOperationException("Encounter is not activated.");


            return (exec.IsCompleted, exec.SecondsInsideZone, 30, exec.CompletionTime);
        }

        public EncounterExecutionStatusDto PingSocialPresence(long userId, long encounterId, double latitude, double longitude)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null || encounter.Type != EncounterType.Social)
                throw new InvalidOperationException("Not a social encounter.");

            var socialEncounter = (SocialEncounter)encounter;
            var requiredPeople = socialEncounter.RequiredPeople;
            var range = socialEncounter.Range;

            var myExecution = _executionRepository.Get(userId, encounterId);
            if (myExecution == null)
                throw new InvalidOperationException("You haven't activated this encounter.");

            myExecution.UpdateLocation(latitude, longitude);
            _executionRepository.Update(myExecution);

            var allExecutions = _executionRepository.GetActiveByEncounter(encounterId);

            var validUsersCount = 0;

            foreach (var exec in allExecutions)
            {
                if ((DateTime.UtcNow - exec.LastActivity).TotalMinutes > 5) continue;

                double distanceToCenter = CalculateDistanceMeters(
                    exec.LastLatitude,
                    exec.LastLongitude,
                    socialEncounter.Location.Latitude,
                    socialEncounter.Location.Longitude
                );

                if (distanceToCenter <= range)
                {
                    validUsersCount++;
                }
            }

            if (validUsersCount >= requiredPeople)
            {
                if (!myExecution.IsCompleted)
                {
                    myExecution.MarkCompleted(socialEncounter.XP.Value);
                    _executionRepository.Update(myExecution);

                    var progress = _touristProgressRepository.GetByUserId(userId)
                                  ?? _touristProgressRepository.Create(new TouristProgress(userId));

                    progress.AddXp(socialEncounter.XP.Value);
                    _touristProgressRepository.Update(progress);
                }
            }

            return new EncounterExecutionStatusDto
            {
                IsCompleted = myExecution.IsCompleted,
                SecondsInsideZone = 0,
                RequiredSeconds = 0,
                CompletionTime = myExecution.CompletionTime?.ToString("O"),
                ActiveTourists = validUsersCount,
                InRange = true
            };
        }
        public bool IsEncounterCompleted(long userId, long encounterId)
        {
            return _executionRepository.IsCompleted(userId, encounterId);
        }
    }
}