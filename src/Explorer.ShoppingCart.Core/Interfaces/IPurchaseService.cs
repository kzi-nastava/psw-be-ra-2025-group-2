using Explorer.ShoppingCart.Core.Dtos;
using FluentResults;

namespace Explorer.ShoppingCart.Core.Interfaces
{
    public interface IPurchaseService
    {
        Result<List<TourPurchaseTokenDto>> CompletePurchase(long touristId);
    }
}
