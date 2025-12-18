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
                return Forbid();
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
            if(id != quiz.Id)
            {
                return BadRequest();
            }

            return Ok(_quizService.Update(User.UserId(), quiz));
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
            catch(ForbiddenException)
            {
                return Forbid();
            }
            catch(ArgumentException)
            {
                return NotFound();
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
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (ForbiddenException)
            {
                return Forbid();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpGet("page-count/{authorId:long}")]
        public ActionResult<int> GetPageCountByAuthor(long authorId, [FromQuery] int pageSize)
        {
            if(User.UserId() != authorId)
            {
                return Forbid();
            }

            return Ok(_quizService.GetPageCountByAuthor(authorId, pageSize));
        }


        [Authorize(Policy = "touristPolicy")]
        [HttpGet]
        public ActionResult<PagedResult<QuizDto>> GetPagedPublishedBlanks([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPagedPublishedBlanks(page, pageSize));
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpGet("{authorId:long}")]
        public ActionResult<PagedResult<QuizDto>> GetPagedPublishedBlanksByAuthor(long authorId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPagedPublishedBlanksByAuthor(authorId, page, pageSize));
        }

        [Authorize(Policy = "touristPolicy")]
        [HttpPost("submit/{id:long}")]
        public ActionResult<QuizDto> Answer(long id, [FromBody] QuizSubmissionDto submission)
        {
            if(submission.QuizId != id)
            {
                return BadRequest();
            }


            /* Nije implementirana submisija resenja - prostor za prosirenje */

            try
            {
                return Ok(_quizService.GetAnswered(id));
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        
        [HttpGet("page-count/published")]
        public ActionResult<int> GetPageCountPublished([FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPageCountPublished(pageSize));
        }

        [HttpGet("page-count/published/{authorId:long}")]
        public ActionResult<int> GetPageCountPublishedByAuthor(long authorId, [FromQuery] int pageSize)
        {
            return Ok(_quizService.GetPageCountByAuthorPublished(authorId, pageSize));
        }
    }
}
