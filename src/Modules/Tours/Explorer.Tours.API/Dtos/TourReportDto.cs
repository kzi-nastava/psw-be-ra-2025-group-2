using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourReportDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public long TouristId { get; set; }
        public string TourName { get; set; }
        public string TouristName { get; set; }
        public string ReportReason { get; set; }
        public string State { get; set; }
    }
}
