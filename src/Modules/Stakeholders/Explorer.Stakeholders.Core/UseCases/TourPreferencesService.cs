using System;
using System.Collections.Generic;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TourPreferencesService : ITourPreferencesService
    {
        private readonly ITourPreferencesRepository _repository;
        private readonly IMapper _mapper;

        public TourPreferencesService(
            ITourPreferencesRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TourPreferencesDto GetByTourist(long touristId)
        {
            var entity = _repository.GetByTouristId(touristId);
            if (entity == null) return null;
            return _mapper.Map<TourPreferencesDto>(entity);
        }

        public TourPreferencesDto Create(TourPreferencesDto dto)
        {
            // Validacija
            ValidateDifficulty(dto.PreferredDifficulty);
            ValidateScore(dto.WalkingScore, nameof(dto.WalkingScore));
            ValidateScore(dto.BicycleScore, nameof(dto.BicycleScore));
            ValidateScore(dto.CarScore, nameof(dto.CarScore));
            ValidateScore(dto.BoatScore, nameof(dto.BoatScore));

            // Kreiraj entity
            var entity = new TourPreferences(
                dto.TouristId, // postavljen u Controller-u iz tokena
                (TourDifficulty)dto.PreferredDifficulty,
                dto.WalkingScore,
                dto.BicycleScore,
                dto.CarScore,
                dto.BoatScore,
                dto.Tags ?? new List<string>()
            );

            var created = _repository.Create(entity);
            return _mapper.Map<TourPreferencesDto>(created);
        }

        public TourPreferencesDto Update(TourPreferencesDto dto)
        {
            // Validacija
            ValidateDifficulty(dto.PreferredDifficulty);
            ValidateScore(dto.WalkingScore, nameof(dto.WalkingScore));
            ValidateScore(dto.BicycleScore, nameof(dto.BicycleScore));
            ValidateScore(dto.CarScore, nameof(dto.CarScore));
            ValidateScore(dto.BoatScore, nameof(dto.BoatScore));

            var entity = _repository.GetByTouristId(dto.TouristId);
            if (entity == null || entity.Id != dto.Id)
            {
                throw new InvalidOperationException("Preferences not found.");
            }

            // Update
            entity.Update(
                (TourDifficulty)dto.PreferredDifficulty,
                dto.WalkingScore,
                dto.BicycleScore,
                dto.CarScore,
                dto.BoatScore,
                dto.Tags ?? new List<string>()
            );

            var updated = _repository.Update(entity);
            return _mapper.Map<TourPreferencesDto>(updated);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        private void ValidateDifficulty(int difficulty)
        {
            if (difficulty < 0 || difficulty > 2)
            {
                throw new ArgumentException("Preferred difficulty must be 0 (Easy), 1 (Medium), or 2 (Hard).");
            }
        }

        private void ValidateScore(int score, string paramName)
        {
            if (score < 0 || score > 3)
            {
                throw new ArgumentException($"{paramName} must be between 0 and 3.", paramName);
            }
        }
    }
}