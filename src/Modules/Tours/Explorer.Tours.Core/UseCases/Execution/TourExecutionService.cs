using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Explorer.Tours.Core.UseCases.Execution
{
    public class TourExecutionService : ITourExecutionService
    {
        private readonly IInternalUserService _userService;

        private readonly ITourExecutionRepository _executionRepository;
        private readonly ITourRepository _tourRepository;

        public TourExecutionService(IInternalUserService userService, ITourExecutionRepository executionRepository, ITourRepository tourRepository)
        {
            _userService = userService;
            _executionRepository = executionRepository;
            _tourRepository = tourRepository;
        }

        public long Proceed(long touristId, long tourId)
        {
            var activeTourId = _userService.GetActiveTourIdByUserId(touristId);

            var tour = _tourRepository.GetByIdAsync(tourId).Result;
            if (tour == null)
                throw new InvalidOperationException($"Cannot find tour with id {tourId}.");

            if(activeTourId == tourId) // Resume
            {
                var execution = _executionRepository.GetActiveByTouristId(touristId);
                if (execution == null || execution.TourId != activeTourId)
                    throw new InvalidDataException("Active tour data is not consistent.");
                
                return execution.Id;
            }
            else
            {
                if(activeTourId == null)
                {
                    var execution = new TourExecution(touristId, tourId, tour.KeyPoints.Count());
                    execution.Start();
                    _userService.SetActiveTourIdByUserId(touristId, tourId);
                    return _executionRepository.Create(execution).Id;
                }
                else
                {
                    throw new InvalidOperationException("Cannot continue a tour while another is in progress.");
                }
            }

        }

        public void Abandon(long touristId, long executionId)
        {
            var execution = _executionRepository.GetActiveByTouristId(touristId);

            if (execution == null || execution.Id != executionId)
                throw new ArgumentException("Invalid arguments.");

            execution.Abandon();
            _userService.ResetActiveTourIdByUserId(touristId);
            _executionRepository.Update(execution);
        }

        public void ArriveAtKeyPointOrdinal(long touristId, long executionId, int keyPointOrdinal)
        {
            var execution = _executionRepository.GetActiveByTouristId(touristId);

            if (execution == null || execution.Id != executionId)
                throw new ArgumentException("Invalid arguments.");

            execution.ArriveAtKeyPointByOrdinal(keyPointOrdinal);
            _executionRepository.Update(execution);
        }

        public void Complete(long touristId,long executionId)
        {
            var execution = _executionRepository.GetActiveByTouristId(touristId);

            if (execution == null || execution.Id != executionId)
                throw new ArgumentException("Invalid arguments.");

            execution.Complete();
            _userService.ResetActiveTourIdByUserId(touristId);
            _executionRepository.Update(execution);
        }

        public TourExecutionDto GetExecutionData(long touristId, long executionId)
        {
            var execution = _executionRepository.Get(executionId);
            var tour = _tourRepository.GetByIdAsync(execution.TourId).Result;

            var keyPointData = new List<KeyPointDto>();

            foreach(var keyPoint in tour.KeyPoints)
            {
                var dto = new KeyPointDto();
                dto.Id = keyPoint.Id;
                dto.Name = keyPoint.Name;
                dto.Description = keyPoint.Description;
                dto.OrdinalNo = keyPoint.OrdinalNo;

                if (execution.HasTouristVisitedKeyPoint(dto.OrdinalNo))
                    dto.SecretText = dto.SecretText;
                else
                    dto.SecretText = null;

                dto.ImageUrl = keyPoint.ImageUrl;
                dto.Latitude = keyPoint.Latitude;
                dto.Longitude = keyPoint.Longitude;

                keyPointData.Add(dto);
            }

            return new TourExecutionDto { Id = executionId, TourId = tour.Id, TouristId = touristId, KeyPointData = keyPointData };
        }
    }
}
