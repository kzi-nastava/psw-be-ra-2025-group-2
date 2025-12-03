using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
namespace Explorer.Tours.API.Public.Administration
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> GetCartForUser(long userId);
        Task ClearCart(long userId);
    }
}