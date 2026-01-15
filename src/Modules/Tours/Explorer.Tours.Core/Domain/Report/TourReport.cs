using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.Report
{
    public enum TourReportState
    {
        Pending,
        Accepted,
        Rejected
    }

    public class TourReport : AggregateRoot
    {
        public long TourId { get; private set; }
        public long TouristId { get; private set; }
        public string ReportReason { get; private set; }
        public TourReportState State { get; private set; }

        protected TourReport() { }

        public TourReport(long tourId, long touristId, string reportReason)
        {
            if (string.IsNullOrWhiteSpace(reportReason))
                throw new ArgumentException("Report reason cannot be empty.");

            TourId = tourId;
            TouristId = touristId;
            ReportReason = reportReason;
            State = TourReportState.Pending;
        }

        public void Accept()
        {
            EnsureIsPending();

            State = TourReportState.Accepted;
        }

        public void Reject()
        {
            EnsureIsPending();

            State = TourReportState.Rejected;
        }

        private void EnsureIsPending()
        {
            if (State != TourReportState.Pending)
            {
                throw new InvalidOperationException($"Cannot change state. Current state is {State}.");
            }
        }
    }
}
