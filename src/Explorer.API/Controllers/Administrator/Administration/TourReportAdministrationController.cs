using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administration/tour-report")]
    [ApiController]
    public class TourReportAdministrationController : ControllerBase
    {
        private readonly ITourReportAdministrationService _reportAdministrationService;

        public TourReportAdministrationController(ITourReportAdministrationService reportAdministrationService)
        {
            _reportAdministrationService = reportAdministrationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TourReportDto>> GetPendingReports()
        {
            return Ok(_reportAdministrationService.GetPendingReports());
        }

        [HttpPut("accept/{id:long}")]
        public IActionResult Accept(long id)
        {
            _reportAdministrationService.AcceptReport(id);
            return Ok();
        }

        [HttpPut("reject/{id:long}")]
        public IActionResult Reject(long id)
        {
            _reportAdministrationService.RejectReport(id);
            return Ok();
        }
    }
}
