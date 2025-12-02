using Explorer.BuildingBlocks.Core.Domain; 

namespace Explorer.ShoppingCart.Core.Domain
{
    public class OrderItem : Entity
    {
        public long TourId { get; private set; }
        public string TourName { get; private set; }
        public decimal Price { get; private set; }

        private OrderItem() { }

        public OrderItem(long tourId, string tourName, decimal price)
        {
            TourId = tourId;
            TourName = tourName;
            Price = price;
        }
    }
}