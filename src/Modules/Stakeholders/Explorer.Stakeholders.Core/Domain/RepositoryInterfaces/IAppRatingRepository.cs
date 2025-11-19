using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IAppRatingRepository
    {
        AppRating Create(AppRating entity);
        AppRating Update(AppRating entity);
        void Delete(long id);
        IEnumerable<AppRating> GetByUserId(long userId);
        PagedResult<AppRating> GetPaged(int page, int pageSize);

         PagedResult<AppRating> GetPagedByUserId(long userId, int page, int pageSize);

    }
}