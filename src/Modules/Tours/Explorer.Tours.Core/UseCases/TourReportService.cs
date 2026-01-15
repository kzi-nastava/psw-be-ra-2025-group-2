using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.UseCases
{
    public class TourReportService : ITourReportService
    {
        private readonly ITourReportRepository _reportRepository;
        private readonly IUsernameProvider _usernameProvider;
        private readonly ITourRepository _tourRepository;

        public TourReportService(ITourReportRepository reportRepository, ITourRepository tourRepository, IUsernameProvider usernameProvider)
        {
            _reportRepository = reportRepository;
            _tourRepository = tourRepository;
            _usernameProvider = usernameProvider;
        }

        public TourReportDto CreateReport(CreateTourReportDto dto)
        {
            var existing = _reportRepository.GetByTouristAndTour(dto.TouristId, dto.TourId);

            var tour = _tourRepository.GetByIdAsync(dto.TourId).Result;
            if (tour == null)
                throw new NotFoundException("Tour not found.");

            var username = _usernameProvider.GetNameById(dto.TouristId) ?? "";

            var report = new TourReport(dto.TourId, dto.TouristId, dto.ReportReason);

            TourReportValidator.ValidateByTourist(report, existing);

            _reportRepository.Create(report);

            return new TourReportDto
            {
                Id = report.Id,
                TourId = report.TourId,
                TouristId = report.TouristId,
                ReportReason = report.ReportReason,
                State = report.State.ToString(),
                TourName = tour.Name,
                TouristName = username
            };
        }
    }
}
