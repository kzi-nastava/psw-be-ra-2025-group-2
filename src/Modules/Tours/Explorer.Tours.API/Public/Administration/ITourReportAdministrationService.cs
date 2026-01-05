using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.Administration
{
    public interface ITourReportAdministrationService
    {
        IEnumerable<TourReportDto> GetPendingReports();
        void AcceptReport(long reportId);
        void RejectReport(long reportId);
    }
}
