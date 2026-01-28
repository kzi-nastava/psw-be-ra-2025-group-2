using System.Security.Claims;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Explorer.API.Controllers.Author
{
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;
        private readonly ITourService _tourService;

        public CouponController(ICouponService couponService, ITourService tourService)
        {
            _couponService = couponService;
            _tourService = tourService;
        }

        private long ExtractAuthorId()
        {
            var idClaim = User.FindFirstValue("id") ?? User.FindFirstValue("personId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(idClaim) || !long.TryParse(idClaim, out var authorId))
            {
                throw new UnauthorizedAccessException("Author ID not found in token.");
            }
            return authorId;
        }

        [AllowAnonymous]
        [HttpGet("api/tourist/coupons/validate/{code}")]
        public ActionResult<CouponDto> ValidateCoupon(string code)
        {
            try
            {
                var coupon = _couponService.GetByCode(code);

                if (coupon.TourId.HasValue)
                {
                    try
                    {
                        var tour = _tourService.Get(coupon.TourId.Value);
                        coupon.TourName = tour?.Name;
                    }
                    catch
                    {
                    }
                }

                return Ok(coupon);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = "Coupon not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal Server Error: {ex.Message}" });
            }
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpGet("api/author/coupons")]
        public ActionResult<List<CouponDto>> GetAll([FromQuery] long? tourId = null)
        {
            try
            {
                var authorId = ExtractAuthorId();
                var coupons = _couponService.GetByAuthor(authorId, tourId);

                foreach (var c in coupons)
                {
                    if (c.TourId.HasValue)
                    {
                        var tour = _tourService.Get(c.TourId.Value);
                        c.TourName = tour?.Name;
                    }
                }

                coupons = coupons.OrderByDescending(c => c.ValidUntil ?? DateTime.MaxValue).ToList();
                return Ok(coupons);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpPost("api/author/coupons")]
        public ActionResult<CouponDto> Create([FromBody] CouponCreateDto dto)
        {
            try
            {
                var authorId = ExtractAuthorId();

                if (dto.TourId.HasValue)
                {
                    TourDto tour;
                    try
                    {
                        tour = _tourService.Get(dto.TourId.Value);
                    }
                    catch (KeyNotFoundException)
                    {
                        return NotFound($"Tour with ID {dto.TourId.Value} not found");
                    }

                    if (tour.AuthorId != authorId)
                        return Forbid();
                }


                return Ok(_couponService.Create(dto, authorId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException) when (dto.TourId.HasValue)
            {
                return NotFound($"Tour with ID {dto.TourId.Value} not found");
            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpPut("api/author/coupons/{code}")]
        public ActionResult<CouponDto> Update(string code, [FromBody] CouponCreateDto dto)
        {
            try
            {
                var authorId = ExtractAuthorId();

                if (dto.TourId.HasValue)
                {
                    TourDto tour;
                    try
                    {
                        tour = _tourService.Get(dto.TourId.Value);
                    }
                    catch (KeyNotFoundException)
                    {
                        return NotFound($"Tour with ID {dto.TourId.Value} not found");
                    }

                    if (tour.AuthorId != authorId)
                    {
                        return Forbid();
                    }
                }

                return Ok(_couponService.Update(code, dto, authorId));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateException) when (dto.TourId.HasValue)
            {
                return NotFound($"Tour with ID {dto.TourId.Value} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpDelete("api/author/coupons/{code}")]
        public IActionResult Delete(string code)
        {
            try
            {
                var authorId = ExtractAuthorId();
                _couponService.Delete(code, authorId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Policy = "authorPolicy")]
        [HttpGet("api/author/coupons/{code}")]
        public ActionResult<CouponDto> GetByCode(string code)
        {
            try
            {
                var result = _couponService.GetByCode(code);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}