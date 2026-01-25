using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class PlanerSuggestionMetadataDto
    {
        long TourId { get; set; }
        string TourName { get; set; }
        int TotalDurationMinutes { get; set; }
        double FirstKeyPointLatitude { get; set; }
        double FirstKeyPointLongitude { get; set; }
        double LastKeyPointLatitude { get; set; }
        double LastKeyPointLongitude { get; set; }
    }
}
