using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.Core.Domain.Execution;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourReviewRepository
{
    PagedResult<TourReview> GetPaged(int page, int pageSize);
    TourReview Get(long id);
    TourReview Create(TourReview review);
    TourReview Update(TourReview review);
    void Delete(long id); // Ako ti treba brisanje

    TourReview? GetByTouristAndTour(long touristId, long tourId);
    List<TourReview> GetAllByTourId(long tourId);
    PagedResult<TourReview> GetByTourIdPaged(int page, int pageSize, long tourId);
}