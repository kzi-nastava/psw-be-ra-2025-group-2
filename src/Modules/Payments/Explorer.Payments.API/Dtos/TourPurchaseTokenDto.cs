namespace Explorer.Stakeholders.API.Dtos
{
    public class TourPurchaseTokenDto
    {
        public long TouristId { get; set; }
        public long TourId { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
