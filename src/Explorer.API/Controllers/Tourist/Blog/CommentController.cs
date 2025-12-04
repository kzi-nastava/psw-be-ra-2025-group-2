using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.API.Controllers.Tourist.Blog
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/blogpost/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ICommentRepository _commentRepository;

        public CommentController(ICommentService commentService, ICommentRepository commentRepository)
        {
            _commentService = commentService;
            _commentRepository = commentRepository;
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == "sub");
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID claim not found.");
            return long.Parse(userIdClaim.Value);
        }

        // GET: api/blogpost/comments
        [HttpGet]
        public ActionResult<List<CommentDto>> GetAll()
        {
            var result = _commentService.GetAll();
            return Ok(result);
        }

        // GET: api/blogpost/comments/5
        [HttpGet("{id:long}")]
        public ActionResult<CommentDto> Get(long id)
        {
            var result = _commentService.Get(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        // GET: api/blogpost/comments/by-post/5
        [HttpGet("by-post/{blogPostId:long}")]
        public ActionResult<List<CommentDto>> GetByBlogPost(long blogPostId)
        {
            var result = _commentService.GetByBlogPost(blogPostId);
            return Ok(result);
        }

        // POST: api/blogpost/comments
        [HttpPost]
        public ActionResult<CommentDto> Create([FromBody] CommentDto dto)
        {
            var currentUserId = GetCurrentUserId();
            dto.UserId = currentUserId; // Setuj userId iz tokena

            var result = _commentService.Create(dto);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT: api/blogpost/comments/5
        [HttpPut("{id:long}")]
        public ActionResult<CommentDto> Update(long id, [FromBody] CommentDto dto)
        {
            var currentUserId = GetCurrentUserId();

            var existingComment = _commentRepository.Get(id);
            if (existingComment == null)
                return NotFound();

            if (existingComment.UserId != currentUserId)
                return Forbid();

            if (!existingComment.CanEditOrDelete())
                return BadRequest("Edit window expired.");

            var result = _commentService.Update(dto, id);
            return Ok(result);
        }

        // DELETE: api/blogpost/comments/5
        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var currentUserId = GetCurrentUserId();

            var comment = _commentRepository.Get(id);
            if (comment == null)
                return NotFound();

            if (comment.UserId != currentUserId)
                return Forbid();

            if (!comment.CanEditOrDelete())
                return BadRequest("Delete window expired.");

            _commentService.Delete(id);
            return NoContent();
        }
    }
}