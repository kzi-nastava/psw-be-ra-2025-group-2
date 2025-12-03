using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class CartItem : ValueObject
    {
        public long TourId { get; private set; }
        public int Quantity { get; private set; }

        public CartItem(long tourId, int quantity)
        {
            TourId = tourId;
            Quantity = quantity;
        }

        public void Increase(int value) => Quantity += value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TourId;
        }
    }
}