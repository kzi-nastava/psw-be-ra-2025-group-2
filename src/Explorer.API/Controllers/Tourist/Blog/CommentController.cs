using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Tours.Core.Domain;
namespace Explorer.Blog.API.Controllers.RegisteredUser
{
    [Authorize(Policy = "RegisteredUserPolicy")]
    [Route("api/registered-user/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public long GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == "sub");
            if (userIdClaim == null) throw new Exception("User ID claim not found.");
            return long.Parse(userIdClaim.Value);
        }


        [HttpGet]
        public ActionResult<List<CommentDto>> GetAll()
        {
            var result = _commentService.GetAll();
            return Ok(result);
        }

        [HttpPost]
        public ActionResult Create([FromBody] CommentDto dto)
        {
            var CurrUserId = GetCurrentUserId();
            CurrUserId = dto.UserId;
            var result = _commentService.Create(dto.UserId, dto.Text);
            return Ok(result);
        }


        [HttpPut]
        public ActionResult Edit([FromBody] CommentDto dto)
        {
            var currentUserId = GetCurrentUserId();
            var result = _commentService.Edit(currentUserId, dto.CreatedAt, dto.Text);
            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            var currentUserId = GetCurrentUserId(); // Poziva servis da obriše komentar po ID-ju (autor je currentUserId)
            id = currentUserId;
            _commentService.Delete(id);
            return NoContent();


        }

    }
}