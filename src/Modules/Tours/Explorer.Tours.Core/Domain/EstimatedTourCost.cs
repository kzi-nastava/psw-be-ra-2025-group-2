using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class EstimatedTourCost : ValueObject
{
    public Money TotalPerPerson { get; private set; }

    private readonly List<EstimatedCostItem> _breakdown = new();
    public IReadOnlyList<EstimatedCostItem> Breakdown => _breakdown.AsReadOnly();

    public bool IsInformational { get; private set; } = true;

    private EstimatedTourCost() { } // EF

    public EstimatedTourCost(Money totalPerPerson, IEnumerable<EstimatedCostItem>? breakdown = null)
    {
        TotalPerPerson = totalPerPerson ?? throw new ArgumentNullException(nameof(totalPerPerson));

        if (breakdown == null) return;

        var list = breakdown.ToList();

        if (list.GroupBy(i => i.Category).Any(g => g.Count() > 1))
            throw new InvalidOperationException("Breakdown cannot contain duplicate categories.");

        if (list.Any(i => i.AmountPerPerson.Currency != TotalPerPerson.Currency))
            throw new InvalidOperationException("All breakdown items must have the same currency as total.");

        _breakdown.AddRange(list);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return TotalPerPerson;
        yield return IsInformational;
        foreach (var item in _breakdown.OrderBy(i => i.Category))
            yield return item;
    }
}
