using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class TourProblem
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        private TourProblem() { }

        public TourProblem(long tourId, string category, string priority, string description)
        {
            TourId = tourId;
            Category = category;
            Priority = priority;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
