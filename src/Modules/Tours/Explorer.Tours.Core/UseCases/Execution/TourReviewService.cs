using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Execution;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Execution;

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

    public TourReviewDto Create(TourReviewDto dto, long touristId)
    {
        var existing = _reviewRepository.GetByTouristAndTour(touristId, dto.TourId);
        if (existing != null)
            throw new InvalidOperationException("You have already reviewed this tour.");

        var execution = GetEligibleExecution(touristId, dto.TourId);

        var tour = _tourRepository.GetByIdAsync(dto.TourId).Result;
        double percentage = 0;
        if (tour.KeyPoints != null && tour.KeyPoints.Count > 0)
            percentage = (double)execution.KeyPointVisits.Count / tour.KeyPoints.Count * 100;
        else
            percentage = 100.0;

        var review = new TourReview(
            dto.TourId,
            dto.Rating,
            dto.Comment,
            touristId,
            execution.Id,            
            DateTime.UtcNow,
            (float)percentage,       
            dto.Images
        );

        var result = _reviewRepository.Create(review);
        return _mapper.Map<TourReviewDto>(result);
    }

    public TourReviewDto Update(TourReviewDto dto, long touristId)
    {
        var existingReview = _reviewRepository.Get(dto.Id);

        if (existingReview == null)
            throw new KeyNotFoundException("Review not found.");

        if (existingReview.TouristId != touristId)
        {
            throw new InvalidOperationException("You can only edit your own reviews.");
        }

        var execution = GetEligibleExecution(touristId, existingReview.TourId);

        var tour = _tourRepository.GetByIdAsync(existingReview.TourId).Result;
        float percentage = 0;
        if (tour.KeyPoints != null && tour.KeyPoints.Count > 0)
            percentage = (float)execution.KeyPointVisits.Count / tour.KeyPoints.Count * 100;
        else
            percentage = 0.0f;

        dto.TouristId = touristId;
        dto.CompletedPercentage = percentage;
        dto.ReviewDate = DateTime.UtcNow;

        var review = _mapper.Map(dto, existingReview);

        var result = _reviewRepository.Update(review);
        return _mapper.Map<TourReviewDto>(result);
    }

    public void Delete(long id, long touristId)
    {
        var tourReview = _reviewRepository.Get(id);

        if (tourReview == null)
            throw new Exception("Tour Review not found.");

        if (tourReview.TouristId != touristId)
            throw new UnauthorizedAccessException("You can only delete your own reviews.");

        _reviewRepository.Delete(id);
    }

    private TourExecution GetEligibleExecution(long touristId, long tourId)
    {
        var execution = _executionRepository.GetExactExecution(touristId, tourId);

        if (execution == null)
            throw new InvalidOperationException("Niste započeli ovu turu, pa je ne možete ni oceniti.");

        var daysSinceActive = (DateTime.UtcNow - execution.LastActivityTimestamp).TotalDays;

        if (daysSinceActive > 7)
            throw new InvalidOperationException("Ne možete ostaviti recenziju jer je prošlo više od nedelju dana od vaše poslednje aktivnosti.");

        var tour = _tourRepository.GetByIdAsync(tourId).Result;

        if (tour == null) throw new InvalidOperationException("Tura ne postoji.");

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
            throw new InvalidOperationException($"Prešli ste samo {percentage:F1}% ture. Morate preći više od 35% da biste ostavili recenziju.");

        return execution;
    }

    public double GetAverageRating(long tourId)
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

    public PagedResult<TourReviewDto> GetByTourId(int page, int pageSize, long tourId)
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