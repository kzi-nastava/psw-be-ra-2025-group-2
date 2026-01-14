using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public enum EstimatedCostCategory
{
    TicketsAndAttractions = 1,
    Transport = 2,
    FoodAndDrink = 3,
    Other = 4
}


public class EstimatedCostItem : ValueObject
{
    public EstimatedCostCategory Category { get; private set; }
    public Money AmountPerPerson { get; private set; }

    private EstimatedCostItem() { } // EF

    public EstimatedCostItem(EstimatedCostCategory category, Money amountPerPerson)
    {
        Category = category;
        AmountPerPerson = amountPerPerson ?? throw new ArgumentNullException(nameof(amountPerPerson));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Category;
        yield return AmountPerPerson;
    }
}
