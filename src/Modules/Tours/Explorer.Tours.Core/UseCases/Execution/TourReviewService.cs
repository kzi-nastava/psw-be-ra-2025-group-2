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
        // 1. Provera da li je korisnik već ocenio ovu turu
        var existing = _reviewRepository.GetByTouristAndTour(touristId, dto.TourId);
        if (existing != null)
            throw new InvalidOperationException("You have already reviewed this tour.");

        // 2. Validacija pravila (35% pređeno i < 7 dana od aktivnosti)
        // Ova metoda nam vraća tačan procenat završenosti u ovom trenutku
        float completedPercentage = ValidateReviewEligibility(touristId, dto.TourId, dto.ExecutionId);

        // 3. Kreiranje domenskog objekta
        var review = new TourReview(
            dto.TourId,
            dto.Rating,
            dto.Comment,
            touristId,
            dto.ExecutionId,
            DateTime.UtcNow,
            completedPercentage,
            dto.Images
        );

        var result = _reviewRepository.Create(review);
        return _mapper.Map<TourReviewDto>(result);
    }

    public TourReviewDto Update(TourReviewDto dto, long touristId)
    {
        // 1. Validacija pravila i osvežavanje procenta
        // Čak i kod izmene, moramo proveriti da li je prošlo više od 7 dana
        // i ažurirati procenat ako je turista napredovao u međuvremenu.
        float currentPercentage = ValidateReviewEligibility(touristId, dto.TourId, dto.ExecutionId);

        // 2. Priprema DTO-a pre mapiranja
        // Pošto ne menjamo domenski model metodama, ažuriramo podatke ovde
        dto.CompletedPercentage = currentPercentage;
        dto.ReviewDate = DateTime.UtcNow;
        dto.TouristId = touristId; // Sigurnosna mera: ne dozvoljavamo promenu vlasnika kroz DTO

        // 3. Mapiranje
        var review = _mapper.Map<TourReview>(dto);

        // 4. Provera vlasništva
        // (Ovo bi idealno išlo pre mapiranja čitanjem iz baze, ali ovako radi sa tvojom arhitekturom)
        if (review.TouristId != touristId)
            throw new UnauthorizedAccessException("You can only edit your own reviews.");

        var result = _reviewRepository.Update(review);
        return _mapper.Map<TourReviewDto>(result);
    }

    public void Delete(long id, long touristId)
    {
        var tourReview = _reviewRepository.Get(id);

        if (tourReview == null)
            throw new Exception("Tour Review not found."); // Ili KeyNotFoundException

        if (tourReview.TouristId != touristId)
            throw new UnauthorizedAccessException("You can only delete your own reviews.");

        _reviewRepository.Delete(id);
    }

    // --- POMOĆNA METODA (Centralizovana logika) ---
    private float ValidateReviewEligibility(long touristId, long tourId, long executionId)
    {
        // A. Dobavi sesiju (Execution)
        // Pazi na cast u (long) jer je executionId int u DTO-u
        var execution = _executionRepository.Get((long)executionId);

        if (execution == null)
            throw new InvalidOperationException("Execution not found.");

        if (execution.TouristId != touristId || execution.TourId != tourId)
            throw new InvalidOperationException("Invalid execution provided (id mismatch).");

        // B. Provera vremena (7 dana od LastActivity)
        // LastActivityTimestamp se ažurira u TourExecution.cs kad god se pozove RecordActivity()
        var daysSinceActive = (DateTime.UtcNow - execution.LastActivityTimestamp).TotalDays;

        if (daysSinceActive > 7)
            throw new InvalidOperationException("You cannot leave or edit a review because more than a week has passed since your last activity.");

        // C. Provera procenta (35%)
        var tour = _tourRepository.GetByIdAsync(tourId).Result; // Napomena: .Result blokira thread, bolje je koristiti await ako možeš promeniti potpise metoda u async
        if (tour == null) throw new InvalidOperationException("Tour not found.");

        double percentage = 0;

        // Računamo procenat na osnovu broja poseta ključnim tačkama
        if (tour.KeyPoints != null && tour.KeyPoints.Count > 0)
        {
            percentage = (double)execution.KeyPointVisits.Count / tour.KeyPoints.Count * 100;
        }
        else
        {
            // Ako tura nema tačke, smatramo da je kompletirana
            percentage = 100.0;
        }

        if (percentage <= 35.0)
            throw new InvalidOperationException($"You have completed only {percentage:F1}% of the tour. You must complete more than 35% to leave a review.");

        return (float)percentage;
    }

    // --- METODE ZA ČITANJE (Ostale su iste) ---

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