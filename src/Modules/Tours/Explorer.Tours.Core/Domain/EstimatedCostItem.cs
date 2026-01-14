using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;


public enum EstimatedCostCategory
{
    TicketsAndAttractions = 0,
    Transport = 1,
    FoodAndDrink = 2,
    Other = 3
}


public class EstimatedCostItem : ValueObject
{
    public EstimatedCostCategory Category { get; private set; }
    public Money AmountPerPerson { get; private set; }

    private EstimatedCostItem() { } // EF

    public EstimatedCostItem(EstimatedCostCategory category, Money amountPerPerson)
    {
        AmountPerPerson = amountPerPerson ?? throw new ArgumentNullException(nameof(amountPerPerson));
        Category = category;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Category;
        yield return AmountPerPerson;
    }
}
