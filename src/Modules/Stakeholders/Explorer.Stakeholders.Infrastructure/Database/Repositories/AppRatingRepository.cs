using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core;
using Explorer.Stakeholders.Core.Domain;


namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class AppRatingRepository : IAppRatingRepository
    {
        private readonly StakeholdersContext _dbContext;

        public AppRatingRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<AppRating> GetByUserId(long userId)
        {
            return _dbContext.AppRatings.Where(r => r.UserId == userId).ToList();
        }


        public AppRating Create(AppRating entity)
        {
            _dbContext.AppRatings.Add(entity);
            _dbContext.SaveChanges();
            return entity;
        }

        public void Delete(long id)
        {
            var entity = _dbContext.AppRatings.Find(id);
            if (entity != null)
            {
                _dbContext.AppRatings.Remove(entity);
                _dbContext.SaveChanges();
            }
        }

        public AppRating Update(AppRating entity)
        {
            _dbContext.AppRatings.Update(entity);
            _dbContext.SaveChanges();
            return entity;
        }



        public PagedResult<AppRating> GetPaged(int page, int pageSize)
        {
            var query = _dbContext.AppRatings.AsQueryable();
            var totalCount = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<AppRating>(items, totalCount);
        }
    }
}