using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class AppRatingService : IAppRatingService
    {
        private readonly IAppRatingRepository _repository;
        private readonly IMapper _mapper;

        public AppRatingService(IAppRatingRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public AppRatingDto Create(AppRatingDto dto)
        {
            var entity = _mapper.Map<AppRating>(dto);
            var result = _repository.Create(entity);
            return _mapper.Map<AppRatingDto>(result);
        }

        public AppRatingDto Update(AppRatingDto dto)
        {
            var entity = _mapper.Map<AppRating>(dto);
            var result = _repository.Update(entity);
            return _mapper.Map<AppRatingDto>(result);
        }

        public void Delete(long id)
        {
            _repository.Delete(id);
        }

        public IEnumerable<AppRatingDto> GetByUserId(long userId)
        {
            var ratings = _repository.GetByUserId(userId);
            return ratings.Select(r => _mapper.Map<AppRatingDto>(r)).ToList();
        }
    }
}