using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration
{
    [Authorize(Policy = "administratorPolicy")]
    [ApiController]
    [Route("api/admin/publicKeyPointRequests")]
    public class AdminPublicKeyPointRequestController : ControllerBase
    {
        private readonly IPublicKeyPointRequestService _service;

        public AdminPublicKeyPointRequestController(IPublicKeyPointRequestService service)
        {
            _service = service;
        }

        [HttpGet("pending")]
        public IActionResult GetPending()
        {
            return Ok(_service.GetPendingRequests());
        }

        [HttpPut("{requestId}/approve")]
        public IActionResult Approve(long requestId)
        {
            _service.ApproveRequest(requestId);
            return Ok();
        }

        [HttpPut("{requestId}/deny")]
        public IActionResult Deny(long requestId, [FromBody] PublicKeyPointRequestDenyDto dto)
        {
            _service.DenyRequest(requestId, dto.Comment);
            return Ok();
        }
    }
}
