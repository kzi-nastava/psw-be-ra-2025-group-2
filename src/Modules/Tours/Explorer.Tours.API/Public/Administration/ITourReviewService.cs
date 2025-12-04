using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface ITourReviewService
{
    PagedResult<TourReviewDto> GetPaged(int page, int pageSize);
    TourReviewDto Create(TourReviewDto review, int touristId);
    TourReviewDto Update(TourReviewDto review, int touristId);
    public void Delete(long id, int touristId);
    double GetAverageRating(int tourId);
    PagedResult<TourReviewDto> GetByTourId(int page, int pageSize, int tourId);
}