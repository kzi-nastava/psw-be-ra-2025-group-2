using Explorer.Tours.Core.Domain.Report;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Unit
{
    public class TourReportTests
    {
        [Fact]
        public void Creates()
        {
            var report = new TourReport(-1, -21, "Invalid tour content");

            report.TourId.ShouldBe(-1);
            report.TouristId.ShouldBe(-21);
            report.ReportReason.ShouldBe("Invalid tour content");
            report.State.ShouldBe(TourReportState.Pending);
        }

        [Fact]
        public void Constructor_fails_empty_reason()
        {
            Should.Throw<ArgumentException>(() => new TourReport(-1, -21, ""));
            Should.Throw<ArgumentException>(() => new TourReport(-1, -21, "   "));
        }

        [Fact]
        public void Accepts()
        {
            var report = new TourReport(-1, -21, "Reason");

            report.Accept();

            report.State.ShouldBe(TourReportState.Accepted);
        }

        [Fact]
        public void Rejects()
        {
            var report = new TourReport(-1, -21, "Reason");

            report.Reject();

            report.State.ShouldBe(TourReportState.Rejected);
        }

        [Fact]
        public void State_transition_fails_if_not_pending()
        {
            var report = new TourReport(-1, -21, "Reason");
            report.Accept();

            Should.Throw<InvalidOperationException>(() => report.Accept());
            Should.Throw<InvalidOperationException>(() => report.Reject());
        }
    }
}
