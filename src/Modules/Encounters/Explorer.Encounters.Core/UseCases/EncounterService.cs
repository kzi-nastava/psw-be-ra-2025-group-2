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
    public class EncounterService : IEncounterService, IInternalEncounterExecutionService, IInternalTouristProgressService, IInternalEncounterStatisticsService
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

            encounter.SetRewards(createDto.FavoriteTourId, createDto.FavoriteBlogId);

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

            var newType = EncounterTypeParser.Parse(updateDto.Type);

            if (encounter.Type != newType)
            {
                _encounterRepository.Delete(updateDto.Id);

                Encounter newEncounter;
                switch (newType)
                {
                    case EncounterType.Social:
                        newEncounter = new SocialEncounter(
                            updateDto.Name,
                            updateDto.Description,
                            new GeoLocation(updateDto.Latitude, updateDto.Longitude),
                            new ExperiencePoints(updateDto.XP),
                            updateDto.RequiredPeople ?? 5,
                            updateDto.Range ?? 10
                        );
                        break;

                    case EncounterType.Location:
                        newEncounter = new HiddenLocationEncounter(
                            updateDto.Name,
                            updateDto.Description,
                            new GeoLocation(updateDto.Latitude, updateDto.Longitude),
                            new ExperiencePoints(updateDto.XP),
                            updateDto.ImageUrl ?? string.Empty,
                            new GeoLocation(updateDto.ImageLatitude ?? 0, updateDto.ImageLongitude ?? 0),
                            updateDto.DistanceTreshold ?? 5
                        );
                        break;

                    case EncounterType.Miscellaneous:
                    default:
                        newEncounter = new MiscEncounter(
                            updateDto.Name,
                            updateDto.Description,
                            new GeoLocation(updateDto.Latitude, updateDto.Longitude),
                            new ExperiencePoints(updateDto.XP)
                        );
                        break;
                }
                var created = _encounterRepository.Create(newEncounter);
                return _mapper.Map<EncounterDto>(created);
            }

            encounter.Update(
                updateDto.Name,
                updateDto.Description,
                new GeoLocation(updateDto.Latitude, updateDto.Longitude),
                new ExperiencePoints(updateDto.XP),
                newType
            );

            if (encounter is HiddenLocationEncounter hidden)
            {
                hidden.UpdateHiddenLocation(
                    updateDto.ImageUrl ?? hidden.ImageUrl,
                    new GeoLocation(updateDto.ImageLatitude ?? hidden.ImageLocation.Latitude,
                                    updateDto.ImageLongitude ?? hidden.ImageLocation.Longitude),
                    updateDto.DistanceTreshold ?? hidden.DistanceTreshold
                );
            }
            else if (encounter is SocialEncounter social)
            {
                social.UpdateSocialEncounter(
                    updateDto.RequiredPeople ?? social.RequiredPeople,
                    updateDto.Range ?? social.Range
                );
            }

            var updated = _encounterRepository.Update(encounter);
            return _mapper.Map<EncounterDto>(updated);
        }
        public void ActivateEncounter(long userId, long encounterId, double latitude, double longitude)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null) throw new NotFoundException("Encounter not found.");

            if (!encounter.IsActive()) throw new InvalidOperationException("Encounter is not active.");

            //if (_executionRepository.IsCompleted(userId, encounterId)) return;

            var existing = _executionRepository.GetUnfinishedByUserId(userId, encounterId);
            if (existing != null && !existing.IsCompleted) return;

            if (encounter.Type == EncounterType.Location)
            {
                var hidden = (HiddenLocationEncounter)encounter;
                var distanceToEncounter = CalculateDistanceMeters(latitude, longitude, hidden.Location.Latitude, hidden.Location.Longitude);

                // STROGA PROVERA: Moraš biti na 20m da bi kliknuo dugme "Activate"
                if (distanceToEncounter > 20)
                {
                    throw new InvalidOperationException("Previše ste daleko. Priđite na 20m da biste aktivirali izazov.");
                }
            }

            _executionRepository.Add(new EncounterExecution(userId, encounterId));
        }
        public (bool IsCompleted, int SecondsInsideZone, int RequiredSeconds, DateTime? CompletionTime, bool IsInRange) PingLocation(
            long userId,
            long encounterId,
            double latitude,
            double longitude,
            int? deltaSeconds = null)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null) throw new NotFoundException("Encounter not found");

            if (encounter.Type != EncounterType.Location)
                throw new InvalidOperationException("Location ping is only supported for Hidden Location encounters.");

            var hidden = encounter as HiddenLocationEncounter;
            if (hidden == null) throw new InvalidOperationException("Not a hidden encounter.");

            var execution = _executionRepository.GetUnfinishedByUserId(userId, encounterId);

            // 1. IZRAČUNAJ DISTANCU DO SLIKE
            var distanceToImage = CalculateDistanceMeters(
                latitude, longitude,
                hidden.ImageLocation.Latitude, hidden.ImageLocation.Longitude);

            // Radijus za sliku (20m ili onaj iz baze)
            double completionRange = hidden.DistanceTreshold > 5 ? hidden.DistanceTreshold : 20;
            bool isInside = distanceToImage <= completionRange;

            // === PAMETNA PROVERA (AUTO-ACTIVATE) ===
            if (execution == null)
            {
                // Ako korisnik nije uspeo da aktivira (jer je bio daleko), a sada je PRIŠAO na 20m -> Aktiviraj mu automatski!
                if (isInside)
                {
                    execution = new EncounterExecution(userId, encounterId);
                    _executionRepository.Add(execution);
                    // Nastavljamo dalje da mu odmah upišemo i prvi sekund...
                }
                else
                {
                    // Ako je još uvek daleko, NE BACAJ GREŠKU 400! 
                    // Vrati "Lažni" status da bi Frontend nastavio da radi (pokazivaće crveno).
                    return (false, 0, HiddenRequiredSecondsInZone, null, false);
                }
            }

            // === OSTATAK JE ISTI ===
            if (execution.IsCompleted)
                return (true, execution.SecondsInsideZone, HiddenRequiredSecondsInZone, execution.CompletionTime, true);

            var ds = deltaSeconds ?? DefaultPingDeltaSeconds;

            execution.RegisterPing(isInside, ds);

            if (!execution.IsCompleted && execution.SecondsInsideZone >= HiddenRequiredSecondsInZone)
            {
                execution.MarkCompleted(hidden.XP.Value);
                var progress = _touristProgressRepository.GetByUserId(userId) ?? _touristProgressRepository.Create(new TouristProgress(userId));
                progress.AddXp(hidden.XP.Value);
                _touristProgressRepository.Update(progress);
            }

            _executionRepository.Update(execution);

            return (execution.IsCompleted, execution.SecondsInsideZone, HiddenRequiredSecondsInZone, execution.CompletionTime, isInside);
        }
        private static double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000;

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

            var execution = _executionRepository.GetUnfinishedByUserId(userId, encounterId);

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
            var progress = _touristProgressRepository.GetByUserId(userId)
                          ?? _touristProgressRepository.Create(new TouristProgress(userId));

            if (progress.Level < 10)
                throw new UnauthorizedAccessException("You must reach level 10 to create challenges.");

            if (progress.Level < 20) createDto.FavoriteTourId = null;

            if (progress.Level < 30) createDto.FavoriteBlogId = null;

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

            var exec = _executionRepository.GetFirstFinishedByUserId(userId, encounterId);

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

            var myExecution = _executionRepository.GetUnfinishedByUserId(userId, encounterId);
            if (myExecution == null)
                throw new InvalidOperationException("You haven't activated this encounter.");

            myExecution.UpdateLocation(latitude, longitude);
            _executionRepository.Update(myExecution);

            var allExecutions = _executionRepository.GetActiveByEncounter(encounterId);

            var validExecutions = new List<EncounterExecution>();

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
                    validExecutions.Add(exec);
                }
            }

            if (validExecutions.Count >= requiredPeople)
            {
                foreach (var validExec in validExecutions)
                {
                    if (!validExec.IsCompleted)
                    {
                        validExec.MarkCompleted(socialEncounter.XP.Value);
                        _executionRepository.Update(validExec);

                        var progress = _touristProgressRepository.GetByUserId(validExec.UserId)
                                        ?? _touristProgressRepository.Create(new TouristProgress(validExec.UserId));

                        progress.AddXp(socialEncounter.XP.Value);
                        _touristProgressRepository.Update(progress);

                        if(validExec.UserId == myExecution.UserId)
                        {
                            myExecution = validExec;
                        }
                    }
                }
            }

            return new EncounterExecutionStatusDto
            {
                IsCompleted = myExecution.IsCompleted,
                SecondsInsideZone = 0,
                RequiredSeconds = 0,
                CompletionTime = myExecution.CompletionTime?.ToString("O"),
                ActiveTourists = validExecutions.Count,
                InRange = true
            };
        }
        public bool IsEncounterCompleted(long userId, long encounterId)
        {
            return _executionRepository.IsCompleted(userId, encounterId);
        }

        public void CancelExecution(long userId, long encounterId)
        {
            var execution = _executionRepository.GetUnfinishedByUserId(userId, encounterId);

            if (execution != null)
            {
                _executionRepository.Delete(execution.Id);
            }
        }

        public List<UserXpDto> GetXpForUsers(IEnumerable<long> userIds)
        {
            var ids = userIds?.Distinct().ToList() ?? new List<long>();
            if (ids.Count == 0) return new List<UserXpDto>();

            // Potrebno: batch metoda u repo (najbolje), ili fallback: loop (ok za mali broj)
            var progresses = _touristProgressRepository.GetByUserIds(ids); // DODATI u repo

            // Ako neko nema TouristProgress, tretiraj kao 0 XP (da svi članovi budu u listi)
            var dict = progresses.ToDictionary(p => p.UserId, p => p);

            return ids.Select(id =>
            {
                if (!dict.TryGetValue(id, out var p))
                    return new UserXpDto { UserId = id, TotalXp = 0, Level = 1 };

                return new UserXpDto { UserId = p.UserId, TotalXp = p.TotalXp, Level = p.Level };
            }).ToList();
        }

        public Dictionary<long, EncounterStatisticsData> GetEncounterStatistics(IEnumerable<long> encounterIds)
        {
            var ids = encounterIds?.Distinct().ToList() ?? new List<long>();
            if (ids.Count == 0) return new Dictionary<long, EncounterStatisticsData>();

            var encounterExecutions = _executionRepository.GetByEncounterIds(ids);


            var stats = encounterExecutions
                .GroupBy(ee => ee.EncounterId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var encounter = _encounterRepository.GetById(g.Key);
                        return new EncounterStatisticsData
                        {
                            EncounterId = g.Key,
                            EncounterName = encounter?.Name ?? "Unknown",
                            TotalAttempts = g.Count(),
                            SuccessfulAttempts = g.Count(e => e.IsCompleted)
                        };
                    }
                );

            return stats;
        }
    }
}