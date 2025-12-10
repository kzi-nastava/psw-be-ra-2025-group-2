namespace Explorer.ShoppingCart.Core.Dtos
{
    public class TourPurchaseTokenDto
    {
        public long TouristId { get; set; }
        public long TourId { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
