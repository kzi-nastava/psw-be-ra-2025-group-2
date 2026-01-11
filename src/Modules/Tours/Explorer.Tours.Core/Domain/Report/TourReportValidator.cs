using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.Report
{
    public static class TourReportValidator
    {
        public static void ValidateByTourist(TourReport newReport, IEnumerable<TourReport> reports)
        {
            foreach(var report in reports)
            {
                if(report.TouristId != newReport.TouristId)
                {
                    throw new ArgumentException("Invalid arguments.");
                }

                if(report.State == TourReportState.Pending && report.TourId == newReport.TourId)
                {
                    throw new InvalidOperationException("Tourist already has pending requests");
                }
            }
        }
    }
}
