using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Internal;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public;
using Explorer.Payments.API.Internal;


namespace Explorer.Tours.Core.UseCases.Administration
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IInternalUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEquipmentRepository? _equipmentRepository;
        private readonly IPublicKeyPointService? _publicKeyPointService;
        private readonly IPublicKeyPointRequestRepository? _requestRepository;
        private readonly ITourExecutionRepository _tourExecutionRepository;
        private readonly IInternalTokenService _internalTokenService;

        public TourService(
            ITourRepository tourRepository,
            IMapper mapper,
            IEquipmentRepository equipmentRepository,
            IInternalUserService userService,
            IPublicKeyPointService publicKeyPointService,
            IPublicKeyPointRequestRepository requestRepository,
            ITourExecutionRepository tourExecutionRepository,
            IInternalTokenService tokenService)
        {
            _tourRepository = tourRepository;
            _userService = userService;
            _mapper = mapper;
            _equipmentRepository = equipmentRepository;
            _publicKeyPointService = publicKeyPointService;
            _requestRepository = requestRepository;
            _tourExecutionRepository = tourExecutionRepository;
            _internalTokenService = tokenService;
        }

        public TourDto Create(CreateTourDto dto)
        {
            var tour = new Tour(dto.Name, dto.Description, dto.Difficulty, dto.AuthorId, dto.Tags);
            tour.SetPrice(dto.Price);


            if (dto.Durations != null)
            {
                foreach (var d in dto.Durations)
                {
                    tour.AddOrUpdateDuration((TransportType)d.TransportType, d.Minutes);
                }
            }

            if (dto.KeyPoints != null && dto.KeyPoints.Any())
            {
                foreach (var kpDto in dto.KeyPoints)
                {
                    var keyPoint = new KeyPoint(
                        kpDto.OrdinalNo,
                        kpDto.Name,
                        kpDto.Description,
                        kpDto.SecretText ?? string.Empty,
                        kpDto.ImageUrl,
                        kpDto.Latitude,
                        kpDto.Longitude,
                        dto.AuthorId,
                        kpDto.EncounterId,
                        kpDto.IsEncounterRequired
                    );
                    tour.AddKeyPoint(keyPoint);
                }
            }

            if (dto.RequiredEquipmentIds.Any())
            {
                if (_equipmentRepository == null)
                    throw new InvalidOperationException("Equipment repository is not configured for this instance of TourService.");

                var requestedEquipment = _equipmentRepository.GetByIdsAsync(dto.RequiredEquipmentIds).Result;

                if (requestedEquipment.Count != dto.RequiredEquipmentIds.Distinct().Count())
                    throw new InvalidOperationException("Some of the selected equipment items do not exist.");

                tour.SetRequiredEquipment(requestedEquipment);
            }

            _tourRepository.AddAsync(tour).Wait();
            return _mapper.Map<TourDto>(tour);
        }

        public IEnumerable<TourDto> GetByAuthor(long authorId)
        {
            var tours = _tourRepository.GetByAuthorAsync(authorId).Result;
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }

        public PagedResult<TourDto> GetByRange(double lat, double lon, int range, int page, int pageSize)
        {
            var tours = _tourRepository.GetAllPublished();
            var filteredTours = tours
                .Where(Tour => Tour.KeyPoints.Any(keypoint => IsWithinRange(lat, lon, keypoint.Latitude, keypoint.Longitude, range * 1000)))
                .ToList();
            var totalCount = filteredTours.Count;

            if (page > 0)
            {
                var pagedResult = filteredTours.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var items = pagedResult.Select(_mapper.Map<TourDto>).ToList();
                return new PagedResult<TourDto>(items, totalCount);
            }
            else
            {
                var items = filteredTours.Select(_mapper.Map<TourDto>).ToList();
                return new PagedResult<TourDto>(items, items.Count);
            }
        }

        private bool IsWithinRange(double latPosition, double lonPosition, double latPoint, double lonPoint, double rangeMeters)
        {
            const double earthRadius = 6371000;
            latPosition *= Math.PI / 180;
            lonPosition *= Math.PI / 180;
            latPoint *= Math.PI / 180;
            lonPoint *= Math.PI / 180;

            double dLat = latPoint - latPosition;
            double dLon = lonPoint - lonPosition;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(latPosition) * Math.Cos(latPoint) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = earthRadius * c;

            return distance <= rangeMeters;
        }

        public TourDto Update(long id, UpdateTourDto dto)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result ?? throw new Exception("Tour not found.");

            tour.Update(dto.Name, dto.Description, dto.Difficulty, dto.Tags);
            tour.SetLength(dto.LengthKm);
            tour.SetPrice(dto.Price);

            if(dto.Durations != null)
{
                tour.ReplaceDurations(dto.Durations.Select(d =>
                    new TourDuration((TransportType)d.TransportType, d.Minutes)
                ));
            }


            _tourRepository.UpdateAsync(tour).Wait();

            return _mapper.Map<TourDto>(tour);
        }

        public void Delete(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result ?? throw new Exception("Tour not found.");

            if (tour.Status != TourStatus.Draft) throw new Exception("Only draft tours can be deleted.");

            if (_publicKeyPointService != null && tour.KeyPoints != null)
            {
                foreach (var keyPoint in tour.KeyPoints)
                {
                    try
                    {
                        _publicKeyPointService.DeleteRequestsBySourceAsync(id, keyPoint.OrdinalNo).Wait();
                    }
                    catch { }
                }
            }

            _tourRepository.DeleteAsync(tour).Wait();
        }

        public async Task<TourDto?> GetByIdAsync(long id, long authorId)
        {
            var tour = await _tourRepository.GetTourWithKeyPointsAsync(id);
            if (tour == null || tour.AuthorId != authorId) return null;

            var tourDto = _mapper.Map<TourDto>(tour);

            if (tourDto.KeyPoints != null && tourDto.KeyPoints.Any())
            {
                foreach (var kpDto in tourDto.KeyPoints)
                {
                    kpDto.PublicStatus = await GetKeyPointPublicStatusAsync(id, kpDto.OrdinalNo);
                }
            }

            return tourDto;
        }

        public async Task<KeyPointDto> AddKeyPoint(long tourId, KeyPointDto dto)
        {
            var tour = await GetTourOrThrowAsync(tourId);

            var keyPoint = new KeyPoint(
                dto.OrdinalNo,
                dto.Name,
                dto.Description,
                dto.SecretText ?? string.Empty,
                dto.ImageUrl,
                dto.Latitude,
                dto.Longitude,
                dto.AuthorId,
                dto.EncounterId,
                dto.IsEncounterRequired
            );

            tour.AddKeyPoint(keyPoint);
            await _tourRepository.UpdateAsync(tour);

            if (dto.SuggestForPublicUse && _publicKeyPointService != null)
            {
                try
                {
                    await _publicKeyPointService.SubmitRequestAsync(tourId, dto.OrdinalNo, dto.AuthorId);
                }
                catch (InvalidOperationException) { }
            }

            var createdKeyPoint = _mapper.Map<KeyPointDto>(keyPoint);
            createdKeyPoint.PublicStatus = await GetKeyPointPublicStatusAsync(tourId, dto.OrdinalNo);

            return createdKeyPoint;
        }

        public async Task<KeyPointDto> UpdateKeyPoint(long tourId, int ordinalNo, KeyPointDto dto)
        {
            var tour = await GetTourOrThrowAsync(tourId);
            var keyPoint = GetKeyPointFromTourOrThrow(tour, ordinalNo);

            keyPoint.Update(
                dto.Name,
                dto.Description,
                dto.SecretText ?? string.Empty,
                dto.ImageUrl,
                dto.Latitude,
                dto.Longitude,
                dto.EncounterId,
                dto.IsEncounterRequired
            );

        

            await _tourRepository.UpdateAsync(tour);

            if (dto.SuggestForPublicUse && _publicKeyPointService != null)
            {
                try
                {
                    await _publicKeyPointService.SubmitRequestAsync(tourId, ordinalNo, dto.AuthorId);
                }
                catch (InvalidOperationException) { }
            }

            var updatedKeyPoint = _mapper.Map<KeyPointDto>(keyPoint);
            updatedKeyPoint.PublicStatus = await GetKeyPointPublicStatusAsync(tourId, ordinalNo);

            return updatedKeyPoint;
        }

        public async Task<KeyPointDto> AddKeyPointImages(long tourId, int ordinalNo, long authorId, List<string> urls)
        {
            var tour = await GetTourOrThrowAsync(tourId);
            if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

            var keyPoint = GetKeyPointFromTourOrThrow(tour, ordinalNo);

            foreach (var url in urls)
                keyPoint.AddImage(url);

            await _tourRepository.UpdateAsync(tour);

            var result = _mapper.Map<KeyPointDto>(keyPoint);
            result.PublicStatus = await GetKeyPointPublicStatusAsync(tourId, ordinalNo);

            return result;
        }

        public async Task SetCoverImage(long tourId, long authorId, string url)
        {
            var tour = await GetTourOrThrowAsync(tourId);
            if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

            tour.SetCoverImage(url);
            await _tourRepository.UpdateAsync(tour);
        }

        public async Task RemoveKeyPointImage(long tourId, int ordinalNo, long authorId, long imageId)
        {
            var tour = await _tourRepository.GetTourWithKeyPointsAndImagesAsync(tourId);
            if (tour == null) throw new KeyNotFoundException("Tour not found.");

            if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

            var kp = GetKeyPointFromTourOrThrow(tour, ordinalNo);

            kp.RemoveImage(imageId);

            await _tourRepository.UpdateAsync(tour);
        }

        public async Task<KeyPointDto> CreateEncounterFromKeyPoint(long tourId, int ordinalNo, KeyPointEncounterDto dto, long authorId)
        {
            var tour = await _tourRepository.GetTourWithKeyPointsAsync(tourId);
            if (tour == null) throw new KeyNotFoundException($"Tour with ID {tourId} not found.");
            if (tour.AuthorId != authorId) throw new UnauthorizedAccessException("You are not the author of this tour.");

            var keyPoint = tour.KeyPoints.FirstOrDefault(kp => kp.OrdinalNo == ordinalNo);
            if (keyPoint == null) throw new KeyNotFoundException($"KeyPoint with OrdinalNo {ordinalNo} not found in tour.");

            throw new NotImplementedException("Integration with Encounter module needed.");
        }

        private async Task<Tour> GetTourOrThrowAsync(long tourId)
        {
            var tour = await _tourRepository.GetTourWithKeyPointsAsync(tourId);
            if (tour == null) throw new KeyNotFoundException($"Tour with ID {tourId} not found.");
            return tour;
        }

        private static KeyPoint GetKeyPointFromTourOrThrow(Tour tour, int ordinalNo)
        {
            var keyPoint = tour.KeyPoints.FirstOrDefault(kp => kp.OrdinalNo == ordinalNo);
            if (keyPoint == null) throw new KeyNotFoundException($"KeyPoint with OrdinalNo {ordinalNo} not found in tour.");
            return keyPoint;
        }

        private async Task<string?> GetKeyPointPublicStatusAsync(long tourId, int ordinalNo)
        {
            if (_requestRepository == null) return null;
            var publicKeyPoint = await _requestRepository.GetPublicKeyPointBySourceAsync(tourId, ordinalNo);
            return publicKeyPoint?.Status.ToString();
        }

        public void RemoveKeyPoint(long tourId, int ordinalNo)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result ?? throw new Exception("Tour not found.");
            tour.RemoveKeyPoint(ordinalNo);
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public void Archive(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result ?? throw new Exception("Tour not found.");
            tour.Archive(DateTime.UtcNow);
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public void Reactivate(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result ?? throw new Exception("Tour not found.");
            tour.Reactivate();
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public List<TourEquipmentItemDto> GetEquipmentForTour(long tourId, long authorId)
        {
            if (_equipmentRepository == null) throw new InvalidOperationException("Equipment repository not configured.");
            var tour = _tourRepository.GetByIdAsync(tourId).Result ?? throw new Exception("Tour not found.");
            if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

            var allEquipment = _equipmentRepository.GetAllAsync().Result;
            return allEquipment.Select(eq => new TourEquipmentItemDto
            {
                Id = eq.Id,
                Name = eq.Name,
                IsRequiredForTour = tour.Equipment.Any(te => te.Id == eq.Id)
            }).ToList();
        }

        public void UpdateEquipmentForTour(long tourId, long authorId, List<long> equipmentIds)
        {
            if (_equipmentRepository == null) throw new InvalidOperationException("Equipment repository not configured.");
            var tour = _tourRepository.GetByIdAsync(tourId).Result ?? throw new Exception("Tour not found.");
            if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();

            var requestedEquipment = _equipmentRepository.GetByIdsAsync(equipmentIds).Result;
            if (requestedEquipment.Count != equipmentIds.Distinct().Count()) throw new InvalidOperationException();

            tour.SetRequiredEquipment(requestedEquipment);
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public List<TourEquipmentItemDto> GetAllEquipmentForAuthor(long authorId)
        {
            if (_equipmentRepository == null) throw new InvalidOperationException("Equipment repository not configured.");
            var allEquipment = _equipmentRepository.GetAllAsync().Result;

            return allEquipment.Select(eq => new TourEquipmentItemDto
            {
                Id = eq.Id,
                Name = eq.Name,
                IsRequiredForTour = false
            }).ToList();
        }

        public TourDto Get(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result ?? throw new KeyNotFoundException("Tour not found");
            return _mapper.Map<TourDto>(tour);
        }

        public TourDto GetPublishedTour(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result ?? throw new KeyNotFoundException("Tour not found");
            if (tour.Status != TourStatus.Published) throw new InvalidOperationException("Tour not published.");
            return _mapper.Map<TourDto>(tour);
        }

        public IEnumerable<TourDto> GetAvailableForTourist(long touristId)
        {
            var availableTourIds = _internalTokenService.GetPurchasedTourIds(touristId);
            var tours = _tourRepository.GetByIds(availableTourIds);
            var dtos = _mapper.Map<IEnumerable<TourDto>>(tours);
            var activeTourId = _userService.GetActiveTourIdByUserId(touristId);

            if (activeTourId.HasValue && tours.All(t => t.Id != activeTourId))
            {
                _userService.ResetActiveTourIdByUserId(touristId);
                activeTourId = null;
            }

            foreach (var dto in dtos)
            {
                dto.IsActive = dto.Id == activeTourId;
                dto.CanBeStarted = activeTourId == null || dto.Id == activeTourId;
                dto.KeyPoints = new List<KeyPointDto>();
            }

            return dtos;
        }

        public TourDto? GetById(long id, long authorId) => GetByIdAsync(id, authorId).Result;

        public void Publish(long tourId, long authorId)
        {
            var tour = _tourRepository.GetTourWithKeyPointsAsync(tourId).Result ?? throw new Exception("Tour not found.");
            if (tour.AuthorId != authorId) throw new UnauthorizedAccessException();
            tour.Publish();
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public PagedResultDto<PublishedTourPreviewDto> GetPublishedForTourist(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 6;

            var tours = _tourRepository.GetAllPublished();
            var totalCount = tours.Count;
            var pageTours = tours.OrderByDescending(t => t.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var results = new List<PublishedTourPreviewDto>();

            foreach (var tour in pageTours)
            {
                var orderedKeyPoints = tour.KeyPoints?.OrderBy(k => k.OrdinalNo).ToList() ?? new List<KeyPoint>();
                var firstKp = orderedKeyPoints.FirstOrDefault();

                var dto = new PublishedTourPreviewDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Difficulty = tour.Difficulty,
                    Price = tour.Price,
                    Tags = tour.Tags?.ToList() ?? new List<string>(),
                    FirstKeyPoint = firstKp != null ? _mapper.Map<KeyPointDto>(firstKp) : null,
                    KeyPointCount = orderedKeyPoints.Count,
                    TotalDurationMinutes = tour.Durations?.Sum(d => d.Minutes) ?? 0,
                    LengthKm = tour.LengthKm,
                    PlaceName = firstKp?.Name,
                    CoverImageUrl = tour.CoverImageUrl,
                    AverageRating = tour.GetAverageRating(),
                    Reviews = tour.Reviews.Select(r =>
                    {
                        var reviewDto = _mapper.Map<TourReviewPublicDto>(r);
                        var u = _userService.GetById(r.TouristId);
                        reviewDto.TouristName = u?.Username ?? "Unknown";
                        return reviewDto;
                    }).ToList()
                };

                results.Add(dto);
            }

            return new PagedResultDto<PublishedTourPreviewDto> { Results = results, TotalCount = totalCount };
        }

        public PagedResultDto<PublishedTourPreviewDto> GetFilteredTours(TourFilterDto filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            int page = filter.Page < 1 ? 1 : filter.Page;
            int pageSize = filter.PageSize < 1 ? 6 : filter.PageSize;

            var tours = _tourRepository.GetAllPublished();

            if (filter.EnvironmentType.HasValue)
                tours = tours.Where(t => t.EnvironmentType == (TourEnvironmentType)filter.EnvironmentType.Value).ToList();

            if (filter.MinPrice.HasValue)
                tours = tours.Where(t => t.Price >= filter.MinPrice.Value).ToList();

            if (filter.MaxPrice.HasValue)
                tours = tours.Where(t => t.Price <= filter.MaxPrice.Value).ToList();

            if (filter.SuitableForList != null && filter.SuitableForList.Any())
            {
                var suitableForEnums = filter.SuitableForList.Select(s => (SuitableFor)s).ToList();
                tours = tours.Where(t => t.SuitableForGroups.Any(sf => suitableForEnums.Contains(sf))).ToList();
            }

            if (filter.FoodTypesList != null && filter.FoodTypesList.Any())
            {
                var foodTypeEnums = filter.FoodTypesList.Select(f => (FoodType)f).ToList();
                tours = tours.Where(t => t.FoodTypes.Any(ft => foodTypeEnums.Contains(ft))).ToList();
            }

            if (filter.AdventureLevelValue.HasValue)
                tours = tours.Where(t => t.AdventureLevel == (AdventureLevel)filter.AdventureLevelValue.Value).ToList();

            if (filter.ActivityTypesList != null && filter.ActivityTypesList.Any())
            {
                var activityTypeEnums = filter.ActivityTypesList.Select(a => (ActivityType)a).ToList();
                tours = tours.Where(t => t.ActivityTypes.Any(at => activityTypeEnums.Contains(at))).ToList();
            }

            var totalCount = tours.Count;
            var pageTours = tours.OrderByDescending(t => t.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            var results = new List<PublishedTourPreviewDto>();

            foreach (var tour in pageTours)
            {
                var orderedKeyPoints = tour.KeyPoints?.OrderBy(k => k.OrdinalNo).ToList() ?? new List<KeyPoint>();
                var firstKp = orderedKeyPoints.FirstOrDefault();

                var dto = new PublishedTourPreviewDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Difficulty = tour.Difficulty,
                    Price = tour.Price,
                    Tags = tour.Tags?.ToList() ?? new List<string>(),
                    FirstKeyPoint = firstKp != null ? _mapper.Map<KeyPointDto>(firstKp) : null,
                    KeyPointCount = orderedKeyPoints.Count,
                    TotalDurationMinutes = tour.Durations?.Sum(d => d.Minutes) ?? 0,
                    LengthKm = tour.LengthKm,
                    PlaceName = firstKp?.Name,
                    CoverImageUrl = tour.CoverImageUrl,
                    AverageRating = tour.GetAverageRating(),
                    Reviews = tour.Reviews.Select(r =>
                    {
                        var reviewDto = _mapper.Map<TourReviewPublicDto>(r);
                        var u = _userService.GetById(r.TouristId);
                        reviewDto.TouristName = u?.Username ?? "Unknown";
                        return reviewDto;
                    }).ToList()
                };

                results.Add(dto);
            }

            return new PagedResultDto<PublishedTourPreviewDto> { Results = results, TotalCount = totalCount };
        }

        public TourReviewDto AddReview(long tourId, long touristId, int rating, string comment, List<string> images)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result ?? throw new KeyNotFoundException("Tour not found.");
            var execution = _tourExecutionRepository.GetExactExecution(touristId, tourId) ?? throw new InvalidOperationException("Tour not started.");

            if ((DateTime.UtcNow - execution.LastActivityTimestamp).TotalDays > 7)
                throw new InvalidOperationException("More than 7 days since last activity.");

            if (execution.GetPercentageCompleted() < 35.0)
                throw new InvalidOperationException("Minimum 35% completion required for review.");

            var review = new TourReview(tourId, touristId, execution.Id, rating, comment, DateTime.UtcNow, (float)execution.GetPercentageCompleted(), images);
            tour.AddReview(review);
            _tourRepository.UpdateAsync(tour).Wait();
            return _mapper.Map<TourReviewDto>(review);
        }

        public TourReviewDto UpdateReview(TourReviewDto reviewDto)
        {
            var tour = _tourRepository.GetByIdAsync(reviewDto.TourId).Result ?? throw new KeyNotFoundException("Tour not found.");
            tour.UpdateReview(reviewDto.TouristId, reviewDto.Rating, reviewDto.Comment, reviewDto.Images);
            _tourRepository.UpdateAsync(tour).Wait();
            return _mapper.Map<TourReviewDto>(tour.Reviews.First(r => r.TouristId == reviewDto.TouristId));
        }

        public void DeleteReview(long touristId, long tourId)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result ?? throw new KeyNotFoundException("Tour not found.");
            tour.DeleteReview(touristId);
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public KeyPointDto AddKeyPointSync(long tourId, KeyPointDto dto) => AddKeyPoint(tourId, dto).Result;

        public KeyPointDto UpdateKeyPointSync(long tourId, int ordinalNo, KeyPointDto dto) => UpdateKeyPoint(tourId, ordinalNo, dto).Result;
    }
}
