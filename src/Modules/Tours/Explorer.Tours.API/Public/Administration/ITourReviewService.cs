using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface ITourReviewService
{
    PagedResult<TourReviewDto> GetPaged(int page, int pageSize);
    TourReviewDto Create(TourReviewDto review, long touristId);
    TourReviewDto Update(TourReviewDto review, long touristId);
    public void Delete(long id, long touristId);
    double GetAverageRating(long tourId);
    PagedResult<TourReviewDto> GetByTourId(int page, int pageSize, long tourId);
}