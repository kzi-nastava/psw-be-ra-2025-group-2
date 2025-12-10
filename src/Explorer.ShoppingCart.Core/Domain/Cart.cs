using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.Core.Domain;

namespace Explorer.ShoppingCart.Core.Domain
{
    public class Cart : AggregateRoot
    {
        public long TouristId { get; private set; }
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public decimal TotalPrice => _items.Sum(item => item.Price);

        private Cart() { }
        public Cart(long touristId) { TouristId = touristId; }

        public void AddItem(Tour tour)
        {
            if (tour.Status != TourStatus.Published)
                throw new InvalidOperationException("The tour is not available for purchase.");

            if (_items.Any(item => item.TourId == tour.Id))
                return;

            _items.Add(new OrderItem(tour.Id, tour.Name, tour.Price));
        }

        public void RemoveItem(long tourId) 
        {
            var itemToRemove = _items.FirstOrDefault(item => item.TourId == tourId);
            if (itemToRemove != null)
            {
                _items.Remove(itemToRemove);
            }
        }
        public void ClearItems()
        {
            _items.Clear();
        }
    }
}