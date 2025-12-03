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

      /*  public Result<PagedResult<TourDto>> GetByRange(double lat, double lon, int range, int type, int page, int pageSize)
        {
            var tours = _tourRepository.GetAllPublished(page, pageSize);
            var filteredTours = tours.Results
                .Where(Tour =>
                {
                    if (type == 1 && Tour.Keypoints.Count > 0)
                    {
                        var firstKeypoint = Tour.Keypoints.First();
                        return isWithinRange(lat, lon, firstKeypoint.Latitude, firstKeypoint.Longitude, range * 1000);
                    }
                    else if (type == 2 && Tour.Keypoints.Count > 0)
                    {
                        var lasttKeypoint = Tour.Keypoints.Last();
                        return isWithinRange(lat, lon, lastKeypoint.Latitude, lastKeypoint.Longitude, range * 1000);
                    }
                    else
                    {
                        return Tour.Keypoints.Any(keypoint => IsWithinRange(lat, lon, keypoint.Latitude, keypoint.Longitude, range * 1000));
                    }

                })
                .ToList();
            var totalCount = filteredTours.Count;

            foreach (var tour in filteredTours)
            {
                if (tour.Keypoints.Count > 1)
                {
                    var firstKeypoint = tour.Keypoints.First();
                    tour.Keypoints.Clear();
                    tour.Keypoints.Add(firstKeypoint);
                }
            }
            var pagedResult = new PagedResult<Tour>(filteredTours, totalCount);
            return MapToDto(pagedResult);

        } */

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
    }
}
