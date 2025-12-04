using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Internal;
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

        public TourService(ITourRepository tourRepository, IInternalUserService userService, IMapper mapper)
        {
            _tourRepository = tourRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public TourDto Create(CreateTourDto dto) 
        {
            var tour = new Tour(dto.Name, dto.Description, dto.Difficulty, dto.AuthorId, dto.Tags);
            _tourRepository.AddAsync(tour).Wait();
            return _mapper.Map<TourDto>(tour);
        }

        public IEnumerable<TourDto> GetByAuthor(long authorId)
        {
            var tours = _tourRepository.GetByAuthorAsync(authorId).Result;
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }

        public TourDto Update(long id, UpdateTourDto dto)
        {
            var tour = _tourRepository.GetByIdAsync(id).Result ?? throw new Exception("Tour not found.");

            tour.Update(dto.Name, dto.Description, dto.Difficulty, dto.Tags);
            
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

        public IEnumerable<TourDto> GetAvailableForTourist(long touristId)
        {
            // TODO refaktorisati kasnije
            var tours = _tourRepository.GetAllAsync().Result;

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

            foreach(var dto in dtos)
            {
                if(activeTourId == null)
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
    }
}
