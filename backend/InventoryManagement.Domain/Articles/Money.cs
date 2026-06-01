namespace InventoryManagement.Domain.Articles;

public readonly record struct Money
{
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Price cannot be negative.", nameof(amount));
        }

        Amount = decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public override string ToString() => Amount.ToString("0.00");
}
