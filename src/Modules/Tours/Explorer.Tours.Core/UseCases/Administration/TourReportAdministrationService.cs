using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class TourReportAdministrationService : ITourReportAdministrationService
    {
        private readonly ITourReportRepository _reportRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IUsernameProvider _usernameProvider;

        public TourReportAdministrationService(ITourReportRepository reportRepository, IUsernameProvider usernameProvider, ITourRepository tourRepository)
        {
            _reportRepository = reportRepository;
            _usernameProvider = usernameProvider;
            _tourRepository = tourRepository;
        }

        public void AcceptReport(long reportId)
        {
            var report = _reportRepository.GetById(reportId);
            if (report == null) throw new NotFoundException("Report not found.");

            report.Accept();

            _reportRepository.Update(report);
        }

        public void RejectReport(long reportId)
        {
            var report = _reportRepository.GetById(reportId);
            if (report == null) throw new NotFoundException("Report not found.");

            report.Reject();

            _reportRepository.Update(report);
        }

        public IEnumerable<TourReportDto> GetPendingReports()
        {
            var reports = _reportRepository.GetPending();

            var totalCount = reports.Count();
            var items = reports
                .Select(r => new TourReportDto
                {
                    Id = r.Id,
                    TourId = r.TourId,
                    TouristId = r.TouristId,
                    TourName = _tourRepository.GetByIdAsync(r.TourId).Result?.Name ?? "",
                    TouristName = _usernameProvider.GetNameById(r.TouristId) ?? "",
                    ReportReason = r.ReportReason,
                    State = r.State.ToString()
                }).ToList();

            return items;
        }
    }
}
