using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;
public class TourReview : Entity
{
    public long TourId { get; private set; }
    public long TouristId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime ReviewDate { get; private set; }
    public float CompletedPercentage { get; private set; }
    public List<string> Images { get; private set; }

    public TourReview() { }

    public TourReview(long tourId, long touristId, int rating, string comment, DateTime reviewDate, float completedPercentage, List<string> images)
    {
        TourId = tourId;
        TouristId = touristId;
        Rating = rating;
        Comment = comment;
        ReviewDate = reviewDate;
        CompletedPercentage = completedPercentage;
        Images = images ?? new List<string>();

        Validate();
    }

    private void Validate()
    {
        if (Rating < 1 || Rating > 5) throw new ArgumentException("Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(Comment)) throw new ArgumentException("Comment cannot be empty.");
        if (TouristId == 0) throw new ArgumentException("Invalid tourist ID.");
    }
    public void Update(int rating, string comment, List<string> images)
    {
        Rating = rating;
        Comment = comment;
        Images = images ?? new List<string>();
        ReviewDate = DateTime.UtcNow; 
        Validate();
    }
}