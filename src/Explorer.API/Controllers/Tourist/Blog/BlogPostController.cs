using Microsoft.AspNetCore.Mvc;
using Explorer.Blog.API.Public;
using Explorer.Blog.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Explorer.API.Controllers.Tourist.Blog
{
    [Route("api/blogpost")]
    [ApiController]
    //[Authorize(Roles = "author,tourist")]
    //[Authorize(Policy = "touristAuthorPolicy")] 
    public class BlogPostController : ControllerBase
    {
        private readonly IBlogPostService _service;

        public BlogPostController(IBlogPostService service)
        {
            _service = service;
        }

        // Kreiranje bloga - dostupno autorima i turistima
        [HttpPost]
        public async Task<ActionResult<BlogPostDto>> Create(CreateBlogPostDto dto)
        {
            long authorId = User.Identity?.IsAuthenticated == true
        ? User.PersonId()
        : -21;
            var created = await _service.CreateAsync(new CreateBlogPostDto
            {
                Title = dto.Title,
                Description = dto.Description,
                ImageUrls = dto.ImageUrls
            }, authorId);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Dohvat bloga po ID - može i neautentifikovani (Published blogovi)
        [HttpGet("{id:long}")]
        [AllowAnonymous]
        public async Task<ActionResult<BlogPostDto>> GetById(long id)
        {
            long? userId = User.Identity?.IsAuthenticated == true ? User.PersonId() : (long?)null;
            var blog = await _service.GetByIdAsync(id, userId);

            if (blog == null)
                return NotFound();

            return Ok(blog);
        }

        // Dohvat svih blogova određenog autora - može i neautentifikovani
        [HttpGet("author/{authorId}")]
        [AllowAnonymous] // ← Dozvoljavamo svima da vide objavljene blogove autora
        public async Task<ActionResult<IEnumerable<BlogPostDto>>> GetByAuthor(long authorId)
        {
            long? userId = User.Identity?.IsAuthenticated == true ? User.PersonId() : (long?)null;
            var blogs = await _service.GetByAuthorAsync(authorId, userId);
            return Ok(blogs);
        }


        // Ažuriranje bloga - samo autor konkretnog bloga
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, UpdateBlogPostDto dto)
        {
            long userId = User.Identity?.IsAuthenticated == true
     ? User.PersonId()
     : -21;

            try
            {
                // Proveri da li korisnik ima pravo da edituje
                var blog = await _service.GetByIdAsync(id, userId);
                if (blog == null)
                    return NotFound();

                if (blog.AuthorId != userId)
                    return Forbid(); // 403 - nije vlasnik bloga

                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // Objavljivanje bloga - samo autor konkretnog bloga
        [HttpPut("{id}/publish")]
        public async Task<IActionResult> Publish(long id)
        {
            long userId = User.Identity?.IsAuthenticated == true
                ? User.PersonId()
                : -21;

            try
            {
                await _service.PublishAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }
    }
}
