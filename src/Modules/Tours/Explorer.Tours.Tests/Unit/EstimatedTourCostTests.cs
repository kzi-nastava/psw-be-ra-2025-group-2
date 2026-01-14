using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.Core.Domain;
using Xunit;

public class EstimatedTourCostTests
{
    [Fact]
    public void Ctor_ShouldThrow_WhenDuplicateCategories()
    {
        // Arrange
        var total = new Money(100, "RSD");
        var items = new[]
        {
            new EstimatedCostItem(EstimatedCostCategory.FoodAndDrink, new Money(10, "RSD")),
            new EstimatedCostItem(EstimatedCostCategory.FoodAndDrink, new Money(20, "RSD")),
        };

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => new EstimatedTourCost(total, items));
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenCurrencyMismatch()
    {
        // Arrange
        var total = new Money(100, "RSD");
        var items = new[]
        {
            new EstimatedCostItem(EstimatedCostCategory.Transport, new Money(10, "EUR"))
        };

        // Act + Assert
        Assert.Throws<InvalidOperationException>(() => new EstimatedTourCost(total, items));
    }

    [Fact]
    public void Ctor_ShouldAllow_EmptyBreakdown()
    {
        // Arrange
        var total = new Money(3500, "RSD");

        // Act
        var cost = new EstimatedTourCost(total, breakdown: null);

        // Assert
        Assert.Equal(3500, cost.TotalPerPerson.Amount);
        Assert.Equal("RSD", cost.TotalPerPerson.Currency);
        Assert.Empty(cost.Breakdown);
    }
}
