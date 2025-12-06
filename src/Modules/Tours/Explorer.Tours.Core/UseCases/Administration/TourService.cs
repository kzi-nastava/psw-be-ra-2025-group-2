using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public TourService(ITourRepository tourRepository, IMapper mapper)
        {
            _tourRepository = tourRepository;
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

        public PagedResult<TourDto> GetByRange(double lat, double lon, int range, int page, int pageSize)
        {
            var tours = _tourRepository.GetAllPublished(page, pageSize);
            var filteredTours = tours
                .Where(Tour =>Tour.KeyPoints.Any(keypoint => IsWithinRange(lat, lon, keypoint.Latitude, keypoint.Longitude, range * 1000)))
                .ToList();
            var totalCount = filteredTours.Count;

          /*  foreach (var tour in filteredTours)
            {
                if (tour.KeyPoints.Count > 1)
                {
                    var firstKeyPoint = tour.KeyPoints.First();
                    tour.KeyPoints.Clear();
                    tour.KeyPoints.Add(firstKeyPoint);
                }
            }*/

            var pagedResult = new PagedResult<Tour>(filteredTours, totalCount);
            var items = pagedResult.Results.Select(_mapper.Map<TourDto>).ToList();
            return new PagedResult<TourDto>(items, pagedResult.TotalCount);

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

    }
}
