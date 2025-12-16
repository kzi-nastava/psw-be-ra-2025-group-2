using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Internal;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;



namespace Explorer.Tours.Core.UseCases.Administration
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _tourRepository;
        private readonly IInternalUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEquipmentRepository? _equipmentRepository;
        private readonly ITourExecutionRepository _tourExecutionRepository;

        /*public TourService(ITourRepository tourRepository, IMapper mapper, IEquipmentRepository equipmentRepository, IInternalUserService userService, ITourExecutionRepository tourExecutionRepository)
        {
            _tourRepository = tourRepository;
            _userService = userService;
            _mapper = mapper;
            _equipmentRepository = equipmentRepository;
        }*/

        public TourService(ITourRepository tourRepository, IMapper mapper, IEquipmentRepository equipmentRepository, IInternalUserService userService, ITourExecutionRepository tourExecutionRepository)
        {
            _tourRepository = tourRepository;
            _userService = userService;
            _mapper = mapper;
            _equipmentRepository = equipmentRepository;
            _tourExecutionRepository = tourExecutionRepository;
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
            
            if ( dto.RequiredEquipmentIds.Any())
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
                .Where(Tour =>Tour.KeyPoints.Any(keypoint => IsWithinRange(lat, lon, keypoint.Latitude, keypoint.Longitude, range * 1000)))
                .ToList();
            var totalCount = filteredTours.Count;

            if(page>0)
            {
                var pagedResult = filteredTours.Skip((page - 1) * pageSize).Take(pageSize).ToList();
                var items = pagedResult.Select(_mapper.Map<TourDto>).ToList();
                return new PagedResult<TourDto>(items, pagedResult.Count);
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

            _tourRepository.DeleteAsync(tour).Wait();
        }

        public TourDto? GetById(long id, long authorId)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result;
            if (tour == null || tour.AuthorId != authorId) return null;

            return _mapper.Map<TourDto>(tour);
        }

        public void AddKeyPoint(long tourId, KeyPointDto dto)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result
                       ?? throw new Exception("Tour not found.");
            tour.AddKeyPoint(_mapper.Map<KeyPoint>(dto));
            _tourRepository.UpdateAsync(tour).Wait();
        }

        public TourDto? GetPublishedTour(long tourId)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result;

            // Vraća turu samo ako postoji i ako je objavljena
            if (tour == null || tour.Status != TourStatus.Published)
            {
                return null;
            }

            return _mapper.Map<TourDto>(tour);
        }

        public void UpdateKeyPoint(long tourId, int ordinalNo, KeyPointDto dto)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result
                       ?? throw new Exception("Tour not found.");

           
            var update = new KeyPointUpdate(
                dto.Name,
                dto.Description,
                dto.SecretText,
                dto.ImageUrl,
                dto.Latitude,
                dto.Longitude
            );

            tour.UpdateKeyPoint(ordinalNo, update);
            _tourRepository.UpdateAsync(tour).Wait();
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

        public IEnumerable<TourDto> GetAvailableForTourist(long touristId)
        {
            // TODO refaktorisati kasnije
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
        // upravljanje statusom ture

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

                dto.AverageRating = tour.GetAverageRating();

                dto.Reviews = tour.Reviews.Select(r =>
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

        public TourReviewDto AddReview(long tourId, long touristId, int rating, string comment, List<string> images)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result;
            if (tour == null) throw new KeyNotFoundException("Tour not found.");

            var execution = _tourExecutionRepository.GetExactExecution(touristId, tourId);

            if (execution == null)
                throw new InvalidOperationException("You haven't started this tour yet.");

            if ((DateTime.UtcNow - execution.LastActivityTimestamp).TotalDays > 7)
                throw new InvalidOperationException("You cannot review this tour because it has been more than 7 days since your last activity.");

            var percentage = execution.GetPercentageCompleted();
            if (percentage < 35.0)
                throw new InvalidOperationException($"You have completed only {percentage:F1}% of the tour. You need at least 35% to leave a review.");

            var review = new TourReview(tourId, touristId, rating, comment, DateTime.UtcNow, (float)percentage, images);

            tour.AddReview(review);

            _tourRepository.UpdateAsync(tour).Wait();

            return _mapper.Map<TourReviewDto>(review);
        }
        public TourReviewDto UpdateReview(TourReviewDto reviewDto)
        {
            var tour = _tourRepository.GetByIdAsync(reviewDto.TourId).Result;
            if (tour == null) throw new KeyNotFoundException("Tour not found.");

            tour.UpdateReview(reviewDto.TouristId, reviewDto.Rating, reviewDto.Comment, reviewDto.Images);

            _tourRepository.UpdateAsync(tour).Wait();

            var updatedReview = tour.Reviews.FirstOrDefault(r => r.TouristId == reviewDto.TouristId);
            return _mapper.Map<TourReviewDto>(updatedReview);
        }

        public void DeleteReview(long touristId, long tourId)
        {
            var tour = _tourRepository.GetByIdAsync(tourId).Result;
            if (tour == null) throw new KeyNotFoundException("Tour not found.");

            tour.DeleteReview(touristId);

            _tourRepository.UpdateAsync(tour).Wait();
        }
    }
}
