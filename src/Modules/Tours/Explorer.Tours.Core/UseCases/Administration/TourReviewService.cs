using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class TourReviewService : ITourReviewService
{
    private readonly ITourReviewRepository _reviewRepository;
    private readonly ITourExecutionRepository _executionRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourReviewService(ITourReviewRepository reviewRepository,
                             ITourExecutionRepository executionRepository,
                             ITourRepository tourRepository,
                             IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _executionRepository = executionRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public TourReviewDto Create(TourReviewDto dto, int touristId)
    {
        long touristIdLong = touristId;
        long tourIdLong = dto.TourId;

        var existing = _reviewRepository.GetByTouristAndTour(touristId, dto.TourId);
        if (existing != null)
            throw new InvalidOperationException("You have already reviewed this tour.");

        var execution = _executionRepository.Get(dto.ExecutionId);

        if (execution.TouristId != touristIdLong || execution.TourId != tourIdLong)
        {
            throw new InvalidOperationException("Invalid execution provided (id mismatch).");
        }

        var tour = _tourRepository.GetByIdAsync(tourIdLong).Result;
        if (tour == null) throw new InvalidOperationException("Tour not found.");

        double percentage = 0;

        if (tour.KeyPoints != null && tour.KeyPoints.Count > 0)
        {
            percentage = (double)execution.KeyPointVisits.Count / tour.KeyPoints.Count * 100;
        }
        else
        {
            percentage = 100.0; 
        }

        if (percentage <= 35.0)
            throw new InvalidOperationException("You must complete more than 35% of the tour to leave a review.");

        var daysSinceActive = (DateTime.UtcNow - execution.LastActivityTimestamp).TotalDays;

        if (daysSinceActive > 7)
            throw new InvalidOperationException("You cannot leave a review because more than a week has passed since your last activity.");

        var review = new TourReview(
            dto.TourId,
            dto.Rating,
            dto.Comment,
            touristId,
            DateTime.UtcNow,
            (float)percentage,
            dto.Images
        );

        var result = _reviewRepository.Create(review);
        return _mapper.Map<TourReviewDto>(result);
    }

    public TourReviewDto Update(TourReviewDto dto, int touristId)
    {
        var review = _mapper.Map<TourReview>(dto);

        if (review.TouristId != touristId)
            throw new UnauthorizedAccessException("You can only edit your own reviews.");

        var result = _reviewRepository.Update(review);

        return _mapper.Map<TourReviewDto>(result);
    }

    public void Delete(long id, int touristId)
    {
        var tourReview = _reviewRepository.Get(id);

        if (tourReview == null)
            throw new Exception("Tour Review not found.");

        if (tourReview.TouristId != touristId)
            throw new UnauthorizedAccessException("You can only delete your own reviews.");

        _reviewRepository.Delete(id);
    }

    public double GetAverageRating(int tourId)
    {
        var reviews = _reviewRepository.GetAllByTourId(tourId);
        if (reviews == null || !reviews.Any()) return 0;
        return reviews.Average(r => r.Rating);
    }

    public PagedResult<TourReviewDto> GetPaged(int page, int pageSize)
    {
        var result = _reviewRepository.GetPaged(page, pageSize);
        return MapToDto(result);
    }

    public PagedResult<TourReviewDto> GetByTourId(int page, int pageSize, int tourId)
    {
        var result = _reviewRepository.GetByTourIdPaged(page, pageSize, tourId);
        return MapToDto(result);
    }

    private PagedResult<TourReviewDto> MapToDto(PagedResult<TourReview> result)
    {
        var items = result.Results.Select(_mapper.Map<TourReviewDto>).ToList();
        return new PagedResult<TourReviewDto>(items, result.TotalCount);
    }
}