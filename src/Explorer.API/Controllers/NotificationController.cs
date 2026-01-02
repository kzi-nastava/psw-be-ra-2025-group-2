using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/notifications")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationController(INotificationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll()
    {
        var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
        var notifications = await _service.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("unread/count")]
    public async Task<ActionResult<object>> GetUnreadCount()
    {
        var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
        var count = await _service.GetUnreadCountAsync(userId);
        return Ok(new { count });
    }

    [HttpPut("{id}/read")]
    public async Task<ActionResult> MarkAsRead(long id)
    {
        try
        {
            var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
            await _service.MarkAsReadAsync(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("read-all")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        var userId = long.Parse(User.FindFirst("id")?.Value ?? "0");
        await _service.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}