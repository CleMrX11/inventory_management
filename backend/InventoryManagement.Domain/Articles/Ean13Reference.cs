namespace InventoryManagement.Domain.Articles;

public readonly record struct Ean13Reference
{
    public string Value { get; }

    public Ean13Reference(string value)
    {
        if (!IsValid(value))
        {
            throw new ArgumentException("Reference must contain exactly 13 digits.", nameof(value));
        }

        Value = value;
    }

    public static bool IsValid(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length == 13
            && value.All(char.IsDigit);
    }

    public override string ToString() => Value;
}
