using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Execution;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using Explorer.Encounters.API.Internal;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Execution
{
    public class TourExecutionService : ITourExecutionService
    {
        private readonly IInternalUserService _userService;
        private readonly ITourExecutionRepository _executionRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IInternalEncounterExecutionService _encounterInternalService;

        public TourExecutionService(IInternalUserService userService, ITourExecutionRepository executionRepository, ITourRepository tourRepository,
            IInternalEncounterExecutionService encounterInternalService)
        {
            _userService = userService;
            _executionRepository = executionRepository;
            _tourRepository = tourRepository;
            _encounterInternalService = encounterInternalService; 
        }

        public TourExecutionDto GetExecution(long touristId, long tourId)
        {
            var execution = _executionRepository.GetExactExecution(touristId, tourId);

            if (execution == null) return null;

            return GetExecutionData(touristId, execution.Id);
        }

        public bool IsFinishedEnough(long touristId, long tourId)
        {
            var execution = _executionRepository.GetExactExecution(touristId, tourId);

            if (execution == null) return false;

            if ((DateTime.UtcNow - execution.LastActivityTimestamp).TotalDays > 7)
            {
                return false;
            }

            return execution.GetPercentageCompleted() >= 35.0;
        }

        public TourExecutionDto Proceed(long touristId, long tourId)
        {
            var activeTourId = _userService.GetActiveTourIdByUserId(touristId);
            var tour = _tourRepository.GetByIdAsync(tourId).Result;

            if (tour == null)
                throw new InvalidOperationException($"Cannot find tour with id {tourId}.");

            if (activeTourId == tourId)
            {
                var execution = _executionRepository.GetActiveByTouristId(touristId);
                if (execution == null || execution.TourId != activeTourId)
                    throw new InvalidDataException("Active tour data is not consistent.");

                execution.RecordActivity();
                _executionRepository.Update(execution);

                return GetExecutionData(touristId, execution.Id);
            }
            else
            {
                if (activeTourId == null || activeTourId == 0)
                {
                    var execution = new TourExecution(touristId, tourId, tour.KeyPoints.Count);

                    execution.Start();

                    _userService.SetActiveTourIdByUserId(touristId, tourId);
                    execution = _executionRepository.Create(execution);

                    return GetExecutionData(touristId, execution.Id);
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

        public void Complete(long touristId, long executionId)
        {
            var execution = _executionRepository.GetActiveByTouristId(touristId);

            if (execution == null || execution.Id != executionId)
                throw new ArgumentException("Invalid arguments.");

            execution.Complete();
            _userService.ResetActiveTourIdByUserId(touristId);
            _executionRepository.Update(execution);
        }

        public KeyPointVisitResponseDto QueryKeyPointVisit(long touristId, long executionId, PositionDto position)
        {
            var execution = _executionRepository.GetActiveByTouristId(touristId);

            if (execution == null || execution.Id != executionId)
                throw new ArgumentException("Invalid arguments.");

            var tour = _tourRepository.GetByIdAsync(execution.TourId).Result;

            if (tour == null)
                throw new InvalidDataException("Invalid tour data.");

            var keyPoints = new List<IKeyPointInfo>(tour.KeyPoints);

            int? visitOrdinal = execution.QueryKeyPointVisit(keyPoints, position.Latitude, position.Longitude);

            _executionRepository.Update(execution);

            return new KeyPointVisitResponseDto { IsNewVisit = visitOrdinal.HasValue, KeyPointOrdinal = visitOrdinal };
        }

        public TourExecutionDto GetExecutionData(long touristId, long executionId)
        {
            var execution = _executionRepository.Get(executionId);
            var tour = _tourRepository.GetByIdAsync(execution.TourId).Result;

            var keyPointData = new List<KeyPointDto>();

            foreach (var keyPoint in tour.KeyPoints.OrderBy(kp => kp.OrdinalNo))
            {
                var dto = new KeyPointDto();
                dto.Id = keyPoint.Id;
                dto.Name = keyPoint.Name;
                dto.Description = keyPoint.Description;
                dto.OrdinalNo = keyPoint.OrdinalNo;
                dto.ImageUrl = keyPoint.ImageUrl;
                dto.Latitude = keyPoint.Latitude;
                dto.Longitude = keyPoint.Longitude;
                dto.EncounterId = keyPoint.EncounterId;
                dto.IsEncounterRequired = keyPoint.IsEncounterRequired;

                bool shouldShowSecret = ShouldShowKeyPointSecret(execution, keyPoint, touristId);

                if (shouldShowSecret)
                    dto.SecretText = keyPoint.SecretText;
                else
                    dto.SecretText = null; 

                keyPointData.Add(dto);
            }

            return new TourExecutionDto
            {
                Id = executionId,
                TourId = tour.Id,
                TouristId = touristId,
                KeyPointData = keyPointData,
                CompletedPercentage = execution.GetPercentageCompleted(),
                LastActivity = execution.LastActivityTimestamp
            };
        }

        private bool ShouldShowKeyPointSecret(TourExecution execution, KeyPoint keyPoint, long touristId)
        {
            if (!execution.ShouldShowKeyPointSecret(keyPoint.OrdinalNo))
                return false;

            if (!keyPoint.EncounterId.HasValue)
                return true;

            if (!keyPoint.IsEncounterRequired)
                return true;

            return _encounterInternalService.IsEncounterCompleted(touristId, keyPoint.EncounterId.Value);
        }
    }
}