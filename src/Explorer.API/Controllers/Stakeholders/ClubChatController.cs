using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [Authorize]
    [ApiController]
    [Route("api/club-chats")]
    public class ClubChatController : ControllerBase
    {
        private readonly IClubChatService _clubChatService;

        public ClubChatController(IClubChatService clubChatService)
        {
            _clubChatService = clubChatService;
        }

        [HttpPost("{clubId:long}/messages")]
        public ActionResult<ClubMessageDto> SendMessage(long clubId, [FromBody] string content)
        {
            var userId = User.PersonId(); 
            var message = _clubChatService.Send(clubId, userId, content);
            return Ok(message);
        }

        [HttpGet("{clubId:long}/messages")]
        public ActionResult<List<ClubMessageDto>> GetMessages(long clubId)
        {
            var userId = User.PersonId();
            var messages = _clubChatService.GetMessages(clubId, userId);
            return Ok(messages);
        }
    }
}
