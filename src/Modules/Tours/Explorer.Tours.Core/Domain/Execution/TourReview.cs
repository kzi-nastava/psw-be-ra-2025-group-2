using Explorer.BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.Execution;

public class TourReview : Entity
{
    public long TourId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; }
    public long TouristId { get; private set; }
    public long ExecutionId { get; private set; }

    public DateTime ReviewDate { get; private set; }
    public float CompletedPercentage { get; private set; }
    public List <string> Images { get; private set; }

    public TourReview()
    {

    }
    public TourReview(long tourId, int rating, string comment, long touristId, long executionId, DateTime reviewDate, float completedPercentage, List<string> images)
    {
        TourId = tourId;
        Rating = rating;
        Comment = comment;
        TouristId = touristId;
        ExecutionId = executionId;
        ReviewDate = reviewDate;
        CompletedPercentage = completedPercentage;
        Images = images ?? new List<string>();

        Validate();
    }

    private void Validate()
    {
        if (Rating < 1 || Rating > 5) throw new ArgumentException("Rating must be a number between 1 and 5.");
        if (string.IsNullOrWhiteSpace(Comment)) throw new ArgumentException("Comment can't be empty.");
        if (TouristId == 0) throw new ArgumentException("Invalid tourist ID.");
        if (TourId == 0) throw new ArgumentException("Invalid tour ID.");
        if (ExecutionId == 0) throw new ArgumentException("Invalid Execution ID.");
    }
}
