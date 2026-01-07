using System;
using System.Collections.Generic;
using System.Security.Claims;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author/bundles")]
    [ApiController]
    public class BundleController : ControllerBase
    {
        private readonly IBundleService _bundleService;

        public BundleController(IBundleService bundleService)
        {
            _bundleService = bundleService;
        }

        // POST: api/author/bundles
        [HttpPost]
        public ActionResult<BundleDto> Create([FromBody] CreateBundleDto dto)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                var result = _bundleService.Create(authorId, dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            // UKLONJEN catch (Exception ex) - ArgumentException će proći kroz!
        }

        // GET: api/author/bundles
        [HttpGet]
        public ActionResult<List<BundleDto>> GetMyBundles()
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            var bundles = _bundleService.GetByAuthorId(authorId);
            return Ok(bundles);
        }

        // GET: api/author/bundles/{id}
        [HttpGet("{id}")]
        public ActionResult<BundleDto> GetById(long id)
        {
            try
            {
                var bundle = _bundleService.GetById(id);
                return Ok(bundle);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: api/author/bundles/{id}
        [HttpPut("{id}")]
        public ActionResult<BundleDto> Update(long id, [FromBody] UpdateBundleDto dto)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                var result = _bundleService.Update(authorId, id, dto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PUT: api/author/bundles/{id}/publish
        [HttpPut("{id}/publish")]
        public IActionResult Publish(long id)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                _bundleService.Publish(authorId, id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/author/bundles/{id}/archive
        [HttpPost("{id}/archive")]
        public IActionResult Archive(long id)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                _bundleService.Archive(authorId, id);
                return Ok(new { message = "Bundle je uspešno arhiviran." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/author/bundles/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var authorIdClaim = User.FindFirst("id");
            if (authorIdClaim == null) return Unauthorized();

            long authorId = long.Parse(authorIdClaim.Value);

            try
            {
                _bundleService.Delete(authorId, id);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}