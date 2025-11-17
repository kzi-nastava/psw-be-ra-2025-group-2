using Microsoft.AspNetCore.Mvc;
using Explorer.Blog.API.Public;
using Explorer.Blog.API.Dtos;

namespace Explorer.API.Controllers.Tourist.Blog
{
    [ApiController]
    [Route("api/blogpost")]
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _service;

        public BlogPostController(IBlogPostService service)
        {
            _service = service;
        }

        // Kreiranje bloga
        [HttpPost]
        public async Task<ActionResult<BlogPostDto>> Create(CreateBlogPostDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Dohvat bloga po ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPostDto>> GetById(long id)
        {
            var blog = await _service.GetByIdAsync(id);
            if (blog == null)
                return NotFound();
            return Ok(blog);
        }

        // Dohvat svih blogova određenog autora
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetByAuthor(long authorId)
        {
            var blogs = await _service.GetByAuthorAsync(authorId);
            return Ok(blogs);
        }

        // Ažuriranje bloga
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UpdateBlogPostDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
    }
}
