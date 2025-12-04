using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum TourStatus
{
    Draft,
    Published,
    Archived
}

public class Tour : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Difficulty { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public TourStatus Status { get; private set; }
    public decimal Price { get; private set; }
    public long AuthorId { get; private set; }
    public List<KeyPoint> KeyPoints { get; private set; } = new();
    public List<TourDuration> Durations { get; private set; } = new();
    public DateTime? PublishedAt { get; private set; }

    public Tour() { }

    public Tour(string name, string description, int difficulty, long authorId, IEnumerable<string>? tags = null)
    {
        if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("Name is required.", nameof(name));
        if(string.IsNullOrEmpty(description)) throw new ArgumentNullException("Description is required.", nameof(description));
        if(difficulty<1 || difficulty>5) throw new ArgumentOutOfRangeException("Difficulty must be between 1 and 5.", nameof(difficulty));
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

    public void SetStatus(TourStatus status) => Status = status;

    public void SetPrice(decimal price)
    {
        if (price < 0) throw new ArgumentException("Price cannot be negative.", nameof(price));
        Price = price;
    }

    public void AddKeyPoint(KeyPoint keyPoint)
    {
        if (keyPoint == null)
            throw new ArgumentNullException(nameof(keyPoint));

        
        if (KeyPoints.Any(k => k.OrdinalNo == keyPoint.OrdinalNo))
            throw new InvalidOperationException($"KeyPoint with OrdinalNo {keyPoint.OrdinalNo} already exists.");

        KeyPoints.Add(keyPoint);
        RecalculateKeyPointOrdinals();
    }

    public void RemoveKeyPoint(int ordinalNo)
    {
        var kp = KeyPoints.FirstOrDefault(k => k.OrdinalNo == ordinalNo);
        if (kp != null)
            KeyPoints.Remove(kp);
    }

    public void UpdateKeyPoint(int ordinalNo, KeyPointUpdate update)
    {
        if (update == null)
            throw new ArgumentNullException(nameof(update));

        var keyPoint = KeyPoints.FirstOrDefault(k => k.OrdinalNo == ordinalNo);
        if (keyPoint == null)
            throw new InvalidOperationException($"KeyPoint with OrdinalNo {ordinalNo} does not exist.");

        var updatedKeyPoint = new KeyPoint(
            ordinalNo,
            update.Name ?? keyPoint.Name,
            update.Description ?? keyPoint.Description,
            update.SecretText ?? keyPoint.SecretText,
            update.ImageUrl ?? keyPoint.ImageUrl,
            update.Latitude,
            update.Longitude
        );

        KeyPoints[KeyPoints.IndexOf(keyPoint)] = updatedKeyPoint;
    }

    public void ClearKeyPoints() => KeyPoints.Clear();

    private void RecalculateKeyPointOrdinals()
    {
        KeyPoints = KeyPoints
            .OrderBy(k => k.OrdinalNo)
            .Select((k, index) => { k.SetOrdinalNo(index + 1); return k; })
            .ToList();
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
}
