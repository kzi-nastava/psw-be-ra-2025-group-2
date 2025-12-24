using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.ShoppingCarts;

public class OrderItem : Entity
{
    public long ShoppingCartId { get; private set; }
    public long TourId { get; private set; }
    public string TourName { get; private set; }
    public Money Price { get; private set; }

    private OrderItem() { }

    public OrderItem(long shoppingCartId, long tourId, string tourName, Money price)
    {
        if (tourId == 0)
            throw new ArgumentException("Invalid TourId");

        if (string.IsNullOrWhiteSpace(tourName))
            throw new ArgumentException("Tour name cannot be empty");

        if (price == null || price.Amount < 0)
            throw new ArgumentException("Invalid price");

        ShoppingCartId = shoppingCartId;
        TourId = tourId;
        TourName = tourName;
        Price = price;
    }
}