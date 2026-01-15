using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Public.DTOs;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/shopping-cart")]
[ApiController]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly ITourService _tourService; 

    public ShoppingCartController(IShoppingCartService shoppingCartService, ITourService tourService)
    {
        _shoppingCartService = shoppingCartService;
        _tourService = tourService; 
    }

    private long ExtractTouristId()
    {
        var idClaim = User.FindFirstValue("id")
                      ?? User.FindFirstValue("personId")
                      ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(idClaim) || !long.TryParse(idClaim, out var touristId))
        {
            throw new UnauthorizedAccessException("Tourist ID not found in token.");
        }
        return touristId;
    }

    [HttpGet]
    public ActionResult<ShoppingCartDto> GetCart()
    {
        try
        {
            var touristId = ExtractTouristId();
            var result = _shoppingCartService.GetCart(touristId);
            return Ok(result);
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

    [HttpPost("items")]
    public ActionResult<ShoppingCartDto> AddItem([FromBody] AddToCartDto dto)
    {
        try
        {
            var touristId = ExtractTouristId();

            var tour = _tourService.GetPublishedTour(dto.TourId);

            if (tour == null)
            {
                return NotFound($"Tour with ID {dto.TourId} not found or not published.");
            }

            var result = _shoppingCartService.AddTourToCart(
                touristId,
                tour.Id,
                tour.Name,
                (double)tour.Price, 
                tour.Tags != null && tour.Tags.Count > 0 ? tour.Tags[0] : "General",
                tour.AuthorId
            );

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message} \n Stack: {ex.StackTrace}");
        }
    }

    [HttpDelete("items/{itemId}")]
    public ActionResult<ShoppingCartDto> RemoveItem(long itemId)
    {
        try
        {
            var touristId = ExtractTouristId();
            var result = _shoppingCartService.RemoveItemFromCart(touristId, itemId);
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

    [HttpDelete]
    public ActionResult ClearCart()
    {
        try
        {
            var touristId = ExtractTouristId();
            _shoppingCartService.ClearCart(touristId);
            return NoContent();
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