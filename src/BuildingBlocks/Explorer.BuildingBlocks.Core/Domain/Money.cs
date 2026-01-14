namespace Explorer.BuildingBlocks.Core.Domain;

public class Money : ValueObject
{
    public const string DefaultCurrency = "AC"; 

    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money() { } // EF

    // Backward-compatible: currency optional
    public Money(decimal amount, string? currency = null)
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");

        Amount = amount;
        Currency = string.IsNullOrWhiteSpace(currency)
            ? DefaultCurrency
            : currency.Trim().ToUpperInvariant();
    }

    // Backward-compatible: (Money.Zero)
    public static Money Zero => new Money(0, DefaultCurrency);

    
    public static Money ZeroIn(string currency) => new Money(0, currency);

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        return new Money(a.Amount - b.Amount, a.Currency);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:F2} {Currency}";
}
