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
            var existingRatings = _repository.GetByUserId(dto.UserId);
            if (existingRatings.Any())
            {
                throw new InvalidOperationException("User has already rated the application.");
            }

            dto.CreatedAt = DateTime.UtcNow;
            var entity = _mapper.Map<AppRating>(dto);
            var result = _repository.Create(entity);
            return _mapper.Map<AppRatingDto>(result);
        }

        public AppRatingDto Update(AppRatingDto dto, long userId, string userRole)
        {
            var ratingToUpdate = _repository.Get(dto.Id);

            if (ratingToUpdate == null)
            {
                throw new KeyNotFoundException($"Rating with ID {dto.Id} not found.");
            }

            if (ratingToUpdate.UserId != userId && userRole != "administrator")
            {
                throw new UnauthorizedAccessException("You are not authorized to update this rating.");
            }

            ratingToUpdate.Update(dto.Score, dto.Comment);
            ratingToUpdate.SetUpdatedAt();

            var result = _repository.Update(ratingToUpdate);
            return _mapper.Map<AppRatingDto>(result);
        }

        public void Delete(long id, long userId, string userRole)
        {
            var ratingToDelete = _repository.Get(id);

            if (ratingToDelete == null)
            {
                throw new KeyNotFoundException($"Rating with ID {id} not found.");
            }

            if (ratingToDelete.UserId != userId && userRole != "administrator")
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this rating.");
            }

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

    }
}