using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos.Quizzes;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Stakeholders
{
    [Authorize(Policy = "touristAuthorPolicy")]
    [Route("api/quizzes")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpGet("full/{authorId:long}")]
        public ActionResult<PagedResult<QuizDto>> GetPagedByAuthor(long authorId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            if(User.UserId() != authorId)
            {
                return Forbid("Forbidden operation - invalid rights.");
            }

            return Ok(_quizService.GetPagedByAuthor(User.UserId(), page, pageSize));
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpPost]
        public ActionResult<QuizDto> Create([FromBody] QuizDto quiz)
        {
            quiz.AuthorId = User.UserId();
            return Ok(_quizService.Create(quiz));
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpPut("{id:long}")]
        public ActionResult<QuizDto> Update(long id, [FromBody] QuizDto quiz)
        {
            if(quiz.Id != id)
            {
                return BadRequest("Invalid request parameters.");
            }

            if(quiz.AuthorId != User.UserId())
            {
                return Forbid("Forbidden operation - invalid rights");
            }

            return Ok(_quizService.Update(quiz));
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            try
            {
                _quizService.Delete(User.UserId(), id);
                return NoContent();
            }
            catch(ForbiddenException fex)
            {
                return Forbid(fex.Message);
            }
            catch(ArgumentException aex)
            {
                return NotFound(aex.Message);
            }
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpPost("publish/{id:long}")]
        public IActionResult Publish(long id)
        {
            try
            {
                _quizService.Publish(User.UserId(), id);
                return Ok();
            }
            catch (InvalidOperationException iex)
            {
                return BadRequest(iex.Message);
            }
            catch (ForbiddenException fex)
            {
                return Forbid(fex.Message);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
        }



        [Authorize(Policy = "touristPolicy")]
        [HttpGet]
        public ActionResult<PagedResult<QuizDto>> GetPagedBlanks([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPagedBlanks(page, pageSize));
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpGet("{authorId:long}")]
        public ActionResult<PagedResult<QuizDto>> GetPagedBlanksByAuthor(long authorId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPagedBlanksByAuthor(authorId, page, pageSize));
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpPost("submit/{id:long}")]
        public ActionResult<QuizDto> Answer(long id, [FromBody] QuizSubmissionDto submission)
        {
            if(submission.QuizId != id)
            {
                return BadRequest("Invalid submission Id.");
            }


            /* Nije implementirana submisija resenja - prostor za prosirenje */

            try
            {
                return Ok(_quizService.GetAnswered(id));
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
        }

        [HttpGet("count")]
        public ActionResult<int> GetPageCount([FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPageCount(pageSize));
        }

        [HttpGet("count/{authorId:long}")]
        public ActionResult<int> GetPageCountByAuthor(long authorId, [FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPageCountByAuthor(authorId, pageSize));
        }
    }
}
