using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;



public class Tour : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Difficulty { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public TourStatus Status { get; private set; }
    public decimal Price { get; private set; }
    public long AuthorId { get; private set; }

    public decimal? LengthKm { get; set; }

    public List<TourDuration> Durations { get; private set; } = new();
    public DateTime? PublishedAt { get; private set; }
    public ICollection<Equipment> Equipment { get; private set; } = new List<Equipment>();
    public DateTime? ArchivedAt { get; private set; }

    public TourEnvironmentType? EnvironmentType { get; private set; }
    public List<FoodType> FoodTypes { get; private set; } = new();
    public AdventureLevel? AdventureLevel { get; private set; }
    public List<ActivityType> ActivityTypes { get; private set; } = new();
    public List<SuitableFor> SuitableForGroups { get; private set; } = new();


    private readonly List<KeyPoint> _keyPoints = new();
    public IReadOnlyList<KeyPoint> KeyPoints => _keyPoints.AsReadOnly();

    private readonly List<TourReview> _reviews = new();
    public IReadOnlyList<TourReview> Reviews => _reviews.AsReadOnly();

    public string CoverImageUrl { get; private set; } = string.Empty;

    public AverageCost? AverageCost { get; private set; }
    public int PurchasesCount { get; private set; } = 0;
    public int StartsCount { get; private set; } = 0;

    public void IncrementPurchases(int delta = 1)
    {
        if (delta <= 0) throw new ArgumentOutOfRangeException(nameof(delta));
        PurchasesCount += delta;
    }

    public void IncrementStarts(int delta = 1)
    {
        if (delta <= 0) throw new ArgumentOutOfRangeException(nameof(delta));
        StartsCount += delta;
    }


    public Tour() { }

    public Tour(string name, string description, int difficulty, long authorId, IEnumerable<string>? tags = null)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Name is required.", nameof(name));
        if (string.IsNullOrEmpty(description)) throw new ArgumentNullException("Description is required.", nameof(description));
        if (difficulty < 1 || difficulty > 5) throw new ArgumentOutOfRangeException("Difficulty must be between 1 and 5.", nameof(difficulty));
        if (authorId == 0) throw new ArgumentOutOfRangeException("AuthorId must be greater than 0.", nameof(authorId));

        Name = name;
        Description = description;
        Difficulty = difficulty;
        AuthorId = authorId;

        if (tags != null)
        {
            Tags = tags.Select(t => t.Trim())
                       .Where(t => !string.IsNullOrWhiteSpace(t))
                       .ToList();
        }

        Status = TourStatus.Draft;
        Price = 0m;
        LengthKm = 0m;

        ArchivedAt = null;
    }

    public void Update(string name, string description, int difficulty, IEnumerable<string>? tags = null)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Archived tours cannot be updated.");

        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (difficulty < 1 || difficulty > 5) throw new ArgumentException("Difficulty must be between 1 and 5.", nameof(difficulty));

        Name = name;
        Description = description;
        Difficulty = difficulty;

        Tags = tags != null ? tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList() : new List<string>();
    }


    public void SetEnvironmentType(TourEnvironmentType? environmentType)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Archived tours cannot be updated.");
        EnvironmentType = environmentType;
    }

    public void SetFoodTypes(IEnumerable<FoodType>? foodTypes)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Archived tours cannot be updated.");
        FoodTypes = foodTypes?.ToList() ?? new List<FoodType>();
    }

    public void SetAdventureLevel(AdventureLevel? adventureLevel)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Archived tours cannot be updated.");
        AdventureLevel = adventureLevel;
    }

    public void SetActivityTypes(IEnumerable<ActivityType>? activityTypes)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Archived tours cannot be updated.");
        ActivityTypes = activityTypes?.ToList() ?? new List<ActivityType>();
    }

    public void SetSuitableForGroups(IEnumerable<SuitableFor>? suitableFor)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Archived tours cannot be updated.");
        SuitableForGroups = suitableFor?.ToList() ?? new List<SuitableFor>();
    }

    public void SetStatus(TourStatus status) => Status = status;

    public void Archive(DateTime now)
    {
        if (Status != TourStatus.Published)
        {
            // Acceptance criteria poruka
            throw new InvalidOperationException("Turu je moguće arhivirati samo ako je u stanju 'objavljena'.");
        }

        Status = TourStatus.Archived;
        ArchivedAt = now;
    }


    public void Reactivate()
    {
        if (Status != TourStatus.Archived)
        {
            // Acceptance criteria poruka
            throw new InvalidOperationException("Ova tura nije arhivirana i ne može se reaktivirati.");
        }

        Status = TourStatus.Published;
        ArchivedAt = null;
    }

    public void SetPrice(decimal price)
    {
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
        Price = price;
    }



    public void AddKeyPoint(KeyPoint keyPoint)
    {
        if (keyPoint == null)
            throw new ArgumentNullException(nameof(keyPoint));

        if (_keyPoints.Any(k => k.OrdinalNo == keyPoint.OrdinalNo))
            throw new InvalidOperationException($"KeyPoint with OrdinalNo {keyPoint.OrdinalNo} already exists.");

        _keyPoints.Add(keyPoint);
        RecalculateKeyPointOrdinals();
    }

    public void RemoveKeyPoint(int ordinalNo)
    {
        var kp = _keyPoints.FirstOrDefault(k => k.OrdinalNo == ordinalNo);
        if (kp != null)
            _keyPoints.Remove(kp);
        UpdateLengthForKeyPoints();
    }

    public void UpdateKeyPoint(int ordinalNo, KeyPointUpdate update)
    {
        if (update == null)
            throw new ArgumentNullException(nameof(update));

        var keyPoint = _keyPoints.FirstOrDefault(k => k.OrdinalNo == ordinalNo);
        if (keyPoint == null)
            throw new InvalidOperationException($"KeyPoint with OrdinalNo {ordinalNo} does not exist.");

        keyPoint.Update(
            update.Name ?? keyPoint.Name,
            update.Description ?? keyPoint.Description,
            update.SecretText ?? keyPoint.SecretText,
            update.ImageUrl ?? keyPoint.ImageUrl,
            update.Latitude,
            update.Longitude,
            update.EncounterId ?? keyPoint.EncounterId,
            update.IsEncounterRequired,
            update.OsmClass ?? keyPoint.OsmClass,
            update.OsmType ?? keyPoint.OsmType
        );
    }

    public void ClearKeyPoints()
    {
        _keyPoints.Clear();
        LengthKm = 0m;
    }

    private void RecalculateKeyPointOrdinals()
    {
        var ordered = _keyPoints.OrderBy(k => k.OrdinalNo).ToList();
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].SetOrdinalNo(i + 1);
        }
        _keyPoints.Clear();
        _keyPoints.AddRange(ordered);
    }

    public void SetRequiredEquipment(IEnumerable<Equipment> equipment)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("Nije moguće menjati opremu za arhiviranu turu.");
        Equipment.Clear();

        if (equipment == null) return;

        foreach (var item in equipment)
        {
            Equipment.Add(item);
        }
    }

    public void Publish()
    {
        if (Status != TourStatus.Draft)
            throw new InvalidOperationException("Only draft tours can be published.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("Tour must have a name before publishing.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new InvalidOperationException("Tour must have a description before publishing.");

        if (Difficulty < 1 || Difficulty > 5)
            throw new InvalidOperationException("Tour must have a valid difficulty before publishing.");

        if (Tags == null || !Tags.Any())
            throw new InvalidOperationException("Tour must have at least one tag before publishing.");

        if (KeyPoints == null || KeyPoints.Count < 2)
            throw new InvalidOperationException("Tour must have at least two key points before publishing.");

        if (Durations == null || !Durations.Any())
            throw new InvalidOperationException("Tour must have at least one duration defined before publishing.");

        Status = TourStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }

    public void SetLength(decimal? lengthKm)
    {
        if (Status == TourStatus.Archived)
            throw new InvalidOperationException("It is not possible to change the length of an archived tour.");

        if (!lengthKm.HasValue)
        {
            LengthKm = null;
            return;
        }

        UpdateLengthForKeyPoints();

        if (lengthKm < 0)
            throw new ArgumentOutOfRangeException(nameof(lengthKm), "Distance cannot be negative.");
        if (lengthKm > 2000)
            throw new ArgumentOutOfRangeException(nameof(lengthKm), "Distance cannot exceed 2000 km.");

        LengthKm = lengthKm;
    }

    private void UpdateLengthForKeyPoints()
    {
        if (_keyPoints.Count < 2)
        {
            LengthKm = 0m;
            return;
        }


    }


    public void AddOrUpdateDuration(TransportType transportType, int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Duration must be positive.");

        var existing = Durations.FirstOrDefault(d => d.TransportType == transportType);
        if (existing != null)
        {
            Durations.Remove(existing);
        }

        Durations.Add(new TourDuration(transportType, minutes));
    }

    public void AddReview(TourReview review)
    {
        if (review == null) throw new ArgumentNullException(nameof(review));

        var existing = _reviews.FirstOrDefault(r => r.TouristId == review.TouristId);
        if (existing != null)
        {
            throw new InvalidOperationException("Tourist has already reviewed this tour.");
        }

        _reviews.Add(review);
    }

    public void UpdateReview(long touristId, int rating, string comment, List<string> images)
    {
        var review = _reviews.FirstOrDefault(r => r.TouristId == touristId);
        if (review == null) throw new InvalidOperationException("Review not found.");

        review.Update(rating, comment, images);
    }

    public void DeleteReview(long touristId)
    {
        var review = _reviews.FirstOrDefault(r => r.TouristId == touristId);
        if (review == null) throw new InvalidOperationException("Review not found.");

        _reviews.Remove(review);
    }

    public double GetAverageRating()
    {
        if (_reviews.Count == 0) return 0;
        return _reviews.Average(r => r.Rating);
    }

    public void SetCoverImage(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Cover image url is required.", nameof(url));

        CoverImageUrl = url;
    }

    public void ClearCoverImage()
    {
        CoverImageUrl = string.Empty;
    }
    public void ReplaceDurations(IEnumerable<TourDuration> durations)
    {
        Durations.Clear();
        Durations.AddRange(durations);
    }
    public void SetAverageCost(AverageCost cost)
    {
        AverageCost = cost ?? throw new ArgumentNullException(nameof(cost));
    }

    public void ClearAverageCost()
    {
        AverageCost = null;
    }


}