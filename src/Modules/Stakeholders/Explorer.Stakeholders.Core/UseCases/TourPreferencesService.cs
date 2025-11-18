using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var entity = _mapper.Map<TourPreferences>(dto);
            var created = _repository.Create(entity);
            return _mapper.Map<TourPreferencesDto>(created);
        }

        public TourPreferencesDto Update(TourPreferencesDto dto)
        {
            var entity = _mapper.Map<TourPreferences>(dto);
            var updated = _repository.Update(entity);
            return _mapper.Map<TourPreferencesDto>(updated);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }
    }
}
