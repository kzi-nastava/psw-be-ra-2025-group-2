using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.ShoppingCarts;

public class OrderItem : Entity
{
    public long ShoppingCartId { get; private set; }

    public long? TourId { get; private set; }

    public long? BundleId { get; private set; }
    public List<long> TourIds { get; private set; } = new();

    // "TOUR" | "BUNDLE"
    public string ItemType { get; private set; } = "TOUR";

    // TourName ili BundleName
    public string Title { get; private set; } = string.Empty;

    public Money Price { get; private set; }
    public long AuthorId { get; private set; }

    private OrderItem() { }

    public OrderItem(long shoppingCartId, long tourId, string title, Money price, long authorId)
    {
        ValidateCommon(shoppingCartId, title, price, authorId);

        if (tourId <= 0)
            throw new ArgumentException("Invalid TourId");

        ShoppingCartId = shoppingCartId;
        TourId = tourId;
        BundleId = null;
        TourIds = new List<long>();
        ItemType = "TOUR";
        Title = title;
        Price = price;
        AuthorId = authorId;
    }

   
    public static OrderItem CreateBundle(
        long shoppingCartId,
        long bundleId,
        string bundleName,
        Money price,
        List<long> tourIds,
        long authorId)
    {
        ValidateCommon(shoppingCartId, bundleName, price, authorId);

        if (bundleId <= 0)
            throw new ArgumentException("Invalid BundleId");

        if (tourIds == null || tourIds.Count == 0 || tourIds.Any(id => id <= 0))
            throw new ArgumentException("Bundle must contain valid tour ids");

        return new OrderItem
        {
            ShoppingCartId = shoppingCartId,
            TourId = null,
            BundleId = bundleId,
            TourIds = tourIds,
            ItemType = "BUNDLE",
            Title = bundleName,
            Price = price,
            AuthorId = authorId
        };
    }

    private static void ValidateCommon(long shoppingCartId, string title, Money price, long authorId)
    {
        if (shoppingCartId <= 0)
            throw new ArgumentException("Invalid ShoppingCartId");

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty");

        if (price == null || price.Amount < 0)
            throw new ArgumentException("Invalid price");

        if (authorId <= 0)
            throw new ArgumentException("Invalid AuthorId");
    }

    public void ApplyDiscount(int discountPercentage)
    {
        if (discountPercentage <= 0 || discountPercentage > 100)
            throw new ArgumentException("Invalid discount percentage");

        var discountAmount = Price.Amount * discountPercentage / 100;
        Price = new Money(Price.Amount - discountAmount, Price.Currency);
    }
}
