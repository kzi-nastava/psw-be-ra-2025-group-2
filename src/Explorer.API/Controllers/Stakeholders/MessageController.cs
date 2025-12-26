using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Dtos.Messages;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [Authorize]
    [ApiController]
    [Route("api/messages")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;
        private readonly IUserService _userService;

        public MessageController(IMessageService service, IUserService userService) 
        {
            _service = service;
            _userService = userService;
        }

        [HttpPost]
        public ActionResult<MessageDto> Send([FromBody] SendMessageDto dto)
        {
            try
            {
                var userId = User.PersonId();
                var result = _service.Send(userId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("conversations/{userId:long}")]
        public ActionResult<List<MessageDto>> Conversation(long userId)
        {
            var me = User.PersonId();
            return Ok(_service.GetConversation(me, userId));
        }

        [HttpGet]
        public ActionResult<List<MessageDto>> GetMyMessages()
        {
            var userId = User.PersonId();
            return Ok(_service.GetAllForUser(userId));
        }

        [HttpPut("{id:long}")]
        public ActionResult<MessageDto> Edit(long id, [FromBody] SendMessageDto dto)
        {
            var userId = User.PersonId();
            return Ok(_service.Edit(userId, id, dto.Content));
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id) 
        {
            var userId = User.PersonId();
            _service.Delete(userId, id);
            return NoContent();
        }

        [HttpGet("users")]
        public ActionResult<List<BasicUserDto>> GetAllUsers()
        {
            try
            {
                var users = _userService.GetAllActiveUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
