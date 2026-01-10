using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Explorer.Payments.Core.Domain.Wallets;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Roles = "tourist")]
    [Route("api/wallet/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private long ExtractTouristId()
        {
            var idClaim = User.FindFirstValue("id")
                ?? User.FindFirstValue("personId")
                ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(idClaim) || !long.TryParse(idClaim, out var touristId))
            {
                throw new UnauthorizedAccessException("Tourist ID not found in token");
            }
            return touristId;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var touristId = ExtractTouristId();
                var notifications = _notificationService.GetTouristNotifications(touristId);
                return Ok(notifications);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("unread")]
        public IActionResult GetUnread()
        {
            try
            {
                var touristId = ExtractTouristId();
                var notifications = _notificationService.GetUnreadNotifications(touristId);
                return Ok(notifications);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPut("{notificationId}/read")]
        public IActionResult MarkAsRead(long notificationId)
        {
            try
            {
                _notificationService.MarkAsRead(notificationId);
                return Ok("Notification marked as read");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
