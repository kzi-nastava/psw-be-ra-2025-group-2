using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-report")]
    [ApiController]
    public class TourReportController : ControllerBase
    {
        private readonly ITourReportService _reportService;

        public TourReportController(ITourReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        public ActionResult<TourReportDto> Create([FromBody] CreateTourReportDto report)
        {
            report.TouristId = User.UserId();
            return Ok(_reportService.CreateReport(report));
        }
    }
}
