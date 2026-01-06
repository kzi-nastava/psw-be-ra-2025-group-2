using Explorer.Tours.Core.Domain.Report;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Unit
{
    public class TourReportValidatorTests
    {
        [Fact]
        public void Validation_passes_valid_data()
        {
            var newReport = new TourReport(-1, -21, "New Reason");
            var existingReports = new List<TourReport>
        {
            new TourReport(-2, -21, "Different tour")
        };

            TourReportValidator.ValidateByTourist(newReport, existingReports);
        }

        [Fact]
        public void Validation_fails_duplicate_pending_tour()
        {
            var newReport = new TourReport(-1, -21, "New Reason");
            var existingReports = new List<TourReport>
        {
            new TourReport(-1, -21, "Old Reason")
        };

            Should.Throw<InvalidOperationException>(() =>
                TourReportValidator.ValidateByTourist(newReport, existingReports));
        }

        [Fact]
        public void Validation_fails_different_tourist_id()
        {
            var newReport = new TourReport(-1, -21, "New Reason");
            var existingReports = new List<TourReport>
        {
            new TourReport(-1, -22, "Someone else's report")
        };

            Should.Throw<ArgumentException>(() =>
                TourReportValidator.ValidateByTourist(newReport, existingReports));
        }
    }
}
