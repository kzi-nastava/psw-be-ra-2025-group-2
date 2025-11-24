using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface IAppRatingService
    {
        AppRatingDto Create(AppRatingDto dto);
        AppRatingDto Update(AppRatingDto dto, long userId, string userRole); 
        void Delete(long id, long userId, string userRole); 
        IEnumerable<AppRatingDto> GetByUserId(long userId);
        PagedResult<AppRatingDto> GetPaged(int page, int pageSize);
    }
}