using Explorer.Stakeholders.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Public
{
    public interface IAppRatingService
    {
        AppRatingDto Create(AppRatingDto dto);
        AppRatingDto Update(AppRatingDto dto);
        void Delete(long id);
        IEnumerable<AppRatingDto> GetByUserId(long userId);
    }
}