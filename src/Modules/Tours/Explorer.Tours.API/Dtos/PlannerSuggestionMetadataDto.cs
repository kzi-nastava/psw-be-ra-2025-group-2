using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class PlannerSuggestionMetadataDto
    {
        public long TourId { get; set; }
        public string TourName { get; set; }
        public int TotalDurationMinutes { get; set; }
        public double FirstKeyPointLatitude { get; set; }
        public double FirstKeyPointLongitude { get; set; }
        public double LastKeyPointLatitude { get; set; }
        public double LastKeyPointLongitude { get; set; }
    }
}
