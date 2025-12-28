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

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IInternalUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEquipmentRepository? _equipmentRepository;
        private readonly ITourReviewRepository? _reviewRepository;
        private readonly IPublicKeyPointService? _publicKeyPointService;
        private readonly IPublicKeyPointRequestRepository? _requestRepository;

        public TourService(
            ITourRepository tourRepository,
            IMapper mapper,
            IEquipmentRepository equipmentRepository,
            IInternalUserService userService,
            ITourReviewRepository reviewRepository,
            IPublicKeyPointService publicKeyPointService,
            IPublicKeyPointRequestRepository requestRepository)
        {
            _tourRepository = tourRepository;
            _userService = userService;
            _mapper = mapper;
            _equipmentRepository = equipmentRepository;
            _reviewRepository = reviewRepository;
            _publicKeyPointService = publicKeyPointService;
            _requestRepository = requestRepository;
        }

        public TourService(
            ITourRepository tourRepository,
            IMapper mapper,
            IEquipmentRepository equipmentRepository,
            IInternalUserService userService)
        {
            _tourRepository = tourRepository;
            _userService = userService;
            _mapper = mapper;
            _equipmentRepository = equipmentRepository;
            _reviewRepository = null;
            _publicKeyPointService = null;
            _requestRepository = null;
        }

        public TourDto Create(CreateTourDto dto)
        {
            var tour = new Tour(dto.Name, dto.Description, dto.Difficulty, dto.AuthorId, dto.Tags);

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
                        dto.AuthorId
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
                    catch (Exception)
                    {
                    }
                }
            }

            _tourRepository.DeleteAsync(tour).Wait();
        }

        public async Task<TourDto> GetByIdAsync(long id, long authorId)
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
                dto.AuthorId
            );

            tour.AddKeyPoint(keyPoint);
            await _tourRepository.UpdateAsync(tour);

            if (dto.SuggestForPublicUse && _publicKeyPointService != null)
            {
                try
                {
                    await _publicKeyPointService.SubmitRequestAsync(tourId, dto.OrdinalNo, dto.AuthorId);
                }
                catch (InvalidOperationException)
                {
                }
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
                dto.Longitude
            );

            await _tourRepository.UpdateAsync(tour);

            if (dto.SuggestForPublicUse && _publicKeyPointService != null)
            {
                try
                {
                    await _publicKeyPointService.SubmitRequestAsync(tourId, ordinalNo, dto.AuthorId);
                }
                catch (InvalidOperationException)
                {
                }
            }

            var updatedKeyPoint = _mapper.Map<KeyPointDto>(keyPoint);
            updatedKeyPoint.PublicStatus = await GetKeyPointPublicStatusAsync(tourId, ordinalNo);

            return updatedKeyPoint;
        }

        private async Task<Tour> GetTourOrThrowAsync(long tourId)
        {
            var tour = await _tourRepository.GetTourWithKeyPointsAsync(tourId);
            if (tour == null)
                throw new KeyNotFoundException($"Tour with ID {tourId} not found.");
            return tour;
        }

        private static KeyPoint GetKeyPointFromTourOrThrow(Tour tour, int ordinalNo)
        {
            var keyPoint = tour.KeyPoints.FirstOrDefault(kp => kp.OrdinalNo == ordinalNo);
            if (keyPoint == null)
                throw new KeyNotFoundException($"KeyPoint with OrdinalNo {ordinalNo} not found in tour.");
            return keyPoint;
        }

        private async Task<string?> GetKeyPointPublicStatusAsync(long tourId, int ordinalNo)
        {
            if (_requestRepository == null) return null;

            var publicKeyPoint = await _requestRepository.GetPublicKeyPointBySourceAsync(tourId, ordinalNo);
            if (publicKeyPoint == null) return null;

            return publicKeyPoint.Status.ToString();
        }

        public void RemoveKeyPoint(long tourId, int ordinalNo)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result
                       ?? throw new Exception("Tour not found.");
            tour.RemoveKeyPoint(ordinalNo);
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public void Archive(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result
                       ?? throw new Exception("Tour not found.");

            tour.Archive(DateTime.UtcNow);

            _tourRepository.UpdateAsync(tour).Wait();
        }

        public void Reactivate(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result
                       ?? throw new Exception("Tour not found.");

            tour.Reactivate();

            _tourRepository.UpdateAsync(tour).Wait();
        }

        public List<TourEquipmentItemDto> GetEquipmentForTour(long tourId, long authorId)
        {
            if (_equipmentRepository == null)
                throw new InvalidOperationException("Equipment repository is not configured for this instance of TourService.");

            var tour = _tourRepository.GetByIdAsync(tourId).Result
                       ?? throw new Exception("Tour not found.");

            if (tour.AuthorId != authorId)
                throw new UnauthorizedAccessException("You are not the author of this tour.");

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
            if (_equipmentRepository == null)
                throw new InvalidOperationException("Equipment repository is not configured for this instance of TourService.");

            var tour = _tourRepository.GetByIdAsync(tourId).Result
                       ?? throw new Exception("Tour not found.");

            if (tour.AuthorId != authorId)
                throw new UnauthorizedAccessException("You are not the author of this tour.");

            var requestedEquipment = _equipmentRepository.GetByIdsAsync(equipmentIds).Result;

            if (requestedEquipment.Count != equipmentIds.Distinct().Count())
                throw new InvalidOperationException("Some of the selected equipment items do not exist.");

            tour.SetRequiredEquipment(requestedEquipment);

            _tourRepository.UpdateAsync(tour).Wait();
        }

        public List<TourEquipmentItemDto> GetAllEquipmentForAuthor(long authorId)
        {
            if (_equipmentRepository == null)
                throw new InvalidOperationException("Equipment repository is not configured for this instance of TourService.");

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
            var tour = _tourRepository.GetByIdAsync(id).Result;
            if (tour == null) throw new KeyNotFoundException("Tour not found: " + id);

            return _mapper.Map<TourDto>(tour);
        }

        public TourDto GetPublishedTour(long id)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result;
            if (tour == null) throw new KeyNotFoundException("Tour not found: " + id);
            if (tour.Status != TourStatus.Published) throw new InvalidOperationException("Tour is not published.");

            return _mapper.Map<TourDto>(tour);
        }

        public IEnumerable<TourDto> GetAvailableForTourist(long touristId)
        {
            var tours = _tourRepository.GetAllNonDrafts();

            var dtos = _mapper.Map<IEnumerable<TourDto>>(tours);

            var activeTourId = _userService.GetActiveTourIdByUserId(touristId);

            if (activeTourId.HasValue)
            {
                var tour = tours.Where(t => t.Id == activeTourId).FirstOrDefault();
                if (tour == null)
                {
                    _userService.ResetActiveTourIdByUserId(touristId);
                    activeTourId = null;
                }
            }

            foreach (var dto in dtos)
            {
                if (activeTourId == null)
                {
                    dto.IsActive = false;
                    dto.CanBeStarted = true;
                }
                else
                {
                    if (dto.Id == activeTourId)
                    {
                        dto.IsActive = true;
                        dto.CanBeStarted = true;
                    }
                    else
                    {
                        dto.IsActive = false;
                        dto.CanBeStarted = false;
                    }
                }
                dto.KeyPoints = new List<KeyPointDto>();
            }

            return dtos;
        }

        public void Publish(long tourId, long authorId)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result
                       ?? throw new Exception("Tour not found.");

            if (tour.AuthorId != authorId)
                throw new UnauthorizedAccessException("You are not authorized to publish this tour.");

            tour.Publish();
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public List<PublishedTourPreviewDto> GetPublishedForTourist()
        {
            if (_reviewRepository == null)
                throw new InvalidOperationException(
                    "ITourReviewRepository is not configured. This method requires reviews.");

            var tours = _tourRepository.GetAllPublished();

            var result = new List<PublishedTourPreviewDto>();

            foreach (var tour in tours)
            {
                var dto = new PublishedTourPreviewDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Difficulty = tour.Difficulty,
                    Price = tour.Price,
                    Tags = tour.Tags?.ToList() ?? new List<string>(),
                    FirstKeyPoint = tour.KeyPoints?
                        .OrderBy(k => k.OrdinalNo)
                        .Select(k => _mapper.Map<KeyPointDto>(k))
                        .FirstOrDefault()
                };

                var reviews = _reviewRepository.GetAllByTourId(tour.Id).ToList();
                dto.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;

                dto.Reviews = reviews.Select(r =>
                {
                    var reviewDto = _mapper.Map<TourReviewPublicDto>(r);
                    var u = _userService.GetById(r.TouristId);
                    reviewDto.TouristName = u?.Username ?? "Unknown";
                    return reviewDto;
                }).ToList();

                result.Add(dto);
            }

            return result;
        }
    }
}