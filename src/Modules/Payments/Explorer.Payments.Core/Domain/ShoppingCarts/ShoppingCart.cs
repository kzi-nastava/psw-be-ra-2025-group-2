using Explorer.BuildingBlocks.Core.Domain;
using System.Text.Json.Serialization;
namespace Explorer.Stakeholders.Core.Domain.ShoppingCarts;
public class ShoppingCart : AggregateRoot
{
    public long TouristId { get; private set; }
    public virtual ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
    public Money TotalPrice { get; private set; } = Money.Zero;
    public ShoppingCart()
    {
        Items = new List<OrderItem>();
    }
    public ShoppingCart(long touristId)
    {
        if (touristId == 0)
            throw new ArgumentException("Invalid TouristId");
        TouristId = touristId;
        TotalPrice = Money.Zero;
        Items = new List<OrderItem>();
    }
    public IReadOnlyCollection<OrderItem> GetItems() => Items.ToList().AsReadOnly();
    public void AddItem(long tourId, string tourName, Money price, string tourStatus)
    {
        if (tourStatus != "Published")
        {
            throw new InvalidOperationException("Tour is not available for purchase");
        }
        if (Items.Any(i => i.TourId == tourId))
        {
            throw new InvalidOperationException("Tour is already in the cart");
        }
        if (price.Amount < 0)
        {
            throw new ArgumentException("Tour price cannot be negative");
        }
        var priceInEur = new Money(price.Amount); 
        var item = new OrderItem(this.Id, tourId, tourName, priceInEur); Items.Add(item);
        RecalculateTotalPrice();
    }
    public void RemoveItem(long itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            throw new KeyNotFoundException("Item not found in cart");
        }
        Items.Remove(item);
        RecalculateTotalPrice();
    }
    public void ClearCart()
    {
        Items.Clear();
        TotalPrice = Money.Zero;
    }
    public int GetItemCount() => Items.Count;
    public bool IsEmpty() => Items.Count == 0;
    private void RecalculateTotalPrice()
    {
        if (!Items.Any())
        {
            TotalPrice = Money.Zero;
            return;
        }
        TotalPrice = Items.Aggregate(new Money(0), (sum, item) => sum + item.Price); 

    }
}