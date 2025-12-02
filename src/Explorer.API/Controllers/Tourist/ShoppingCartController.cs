using AutoMapper;
using Explorer.ShoppingCart.Core.Dtos;
using Explorer.ShoppingCart.Core.Interfaces;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/shopping-cart")]
    public class ShoppingCartController : ControllerBase 
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IMapper _mapper;

        public ShoppingCartController(IShoppingCartService shoppingCartService, IMapper mapper)
        {
            _shoppingCartService = shoppingCartService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<ShoppingCartDto> GetCart()
        {
            var result = _shoppingCartService.GetByTouristId(User.PersonId());

            if (result.IsFailed)
            {
                return BadRequest(result.Errors);
            }

            return Ok(_mapper.Map<ShoppingCartDto>(result.Value));
        }

        [HttpPost("add/{tourId:long}")]
        public async Task<ActionResult<ShoppingCartDto>> AddItem(long tourId)
        {
            try
            {
                var result = await _shoppingCartService.AddItem(User.PersonId(), tourId);

                if (result.IsFailed)
                {
                    return BadRequest(result.Errors);
                }

                return Ok(_mapper.Map<ShoppingCartDto>(result.Value));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    ExceptionType = ex.GetType().Name,
                    StackTrace = ex.StackTrace
                });
            }
        }


        [HttpDelete("remove/{tourId:long}")]
        public ActionResult<ShoppingCartDto> RemoveItem(long tourId)
        {
            var result = _shoppingCartService.RemoveItem(User.PersonId(), tourId);

            if (result.IsFailed)
            {
                return BadRequest(result.Errors);
            }

            return Ok(_mapper.Map<ShoppingCartDto>(result.Value));
        }
    }
}