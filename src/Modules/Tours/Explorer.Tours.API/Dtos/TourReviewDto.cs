using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class TourReviewDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public long ExecutionId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public long TouristId { get; set; }
    public DateTime ReviewDate { get; set; }
    public float CompletedPercentage { get; set; }
    public List<string> Images { get; set; }

}
