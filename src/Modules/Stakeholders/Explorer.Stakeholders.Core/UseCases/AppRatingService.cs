using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
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
            dto.CreatedAt = DateTime.UtcNow; 
            var entity = _mapper.Map<AppRating>(dto);
            var result = _repository.Create(entity);
            return _mapper.Map<AppRatingDto>(result);
        }

        public AppRatingDto Update(AppRatingDto dto)
        {
            var existingRating = _repository.GetByUserId(dto.UserId).FirstOrDefault();

            if (existingRating == null)
            {
                return Create(dto);
            }
            else
            {
                _mapper.Map(dto, existingRating);

                existingRating.SetUpdatedAt();

                var result = _repository.Update(existingRating);

                return _mapper.Map<AppRatingDto>(result);
            }
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


        public PagedResult<AppRatingDto> GetPaged(int page, int pageSize)
        {

            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }


            var pagedEntities = _repository.GetPaged(page, pageSize);

            var dtoList = _mapper.Map<List<AppRatingDto>>(pagedEntities.Results);

            return new PagedResult<AppRatingDto>(dtoList, pagedEntities.TotalCount);
        }

        public PagedResult<AppRatingDto> GetPagedByUserId(long userId, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var pagedEntities = _repository.GetPagedByUserId(userId, page, pageSize);
            var dtoList = _mapper.Map<List<AppRatingDto>>(pagedEntities.Results);

            return new PagedResult<AppRatingDto>(dtoList, pagedEntities.TotalCount);
        }

    }


}