using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.ShoppingCarts;

public class OrderItem : Entity
{
    public long ShoppingCartId { get; private set; }
    public long TourId { get; private set; }
    public string TourName { get; private set; }
    public Money Price { get; private set; }
    public long AuthorId { get; private set; }

    private OrderItem() { }

    public OrderItem(long shoppingCartId, long tourId, string tourName, Money price, long authorId)
    {
        if (tourId == 0)
            throw new ArgumentException("Invalid TourId");

        if (string.IsNullOrWhiteSpace(tourName))
            throw new ArgumentException("Tour name cannot be empty");

        if (price == null || price.Amount < 0)
            throw new ArgumentException("Invalid price");

        if (authorId == 0)
            throw new ArgumentException("Invalid AuthorId");

        ShoppingCartId = shoppingCartId;
        TourId = tourId;
        TourName = tourName;
        Price = price;
        AuthorId = authorId;
    }

    public void ApplyDiscount(int discountPercentage)
    {
        if(discountPercentage <= 0 || discountPercentage > 100)
        {
            throw new ArgumentException("Invalid discount percentage");
        }

        var discountAmount = Price.Amount * discountPercentage / 100;
        Price = new Money( Price.Amount - discountAmount, Price.Currency);
    }
}