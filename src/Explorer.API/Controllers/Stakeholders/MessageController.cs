using Explorer.Stakeholders.API.Dtos.Messages;
using Explorer.Stakeholders.API.Public;
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

        public MessageController(IMessageService service) 
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<MessageDto> Send([FromBody] SendMessageDto dto)
        {
            //var userId = User.PersonId();
            //var result = _service.Send(userId, dto);
            //return Ok(result);
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
    }
}
