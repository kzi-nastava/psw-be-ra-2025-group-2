using Explorer.Stakeholders.API.Dtos;
namespace Explorer.Stakeholders.API.Public
{
    public interface IPurchaseService
    {
        List<TourPurchaseTokenDto> CompletePurchase(long touristId);
    }
}
