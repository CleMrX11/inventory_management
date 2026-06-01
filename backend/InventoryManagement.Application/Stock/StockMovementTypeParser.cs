using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Application.Stock;

internal static class StockMovementTypeParser
{
    public static StockMovementType Parse(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Stock movement type must be receive, remove, or adjust.", nameof(value));
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "receive" => StockMovementType.Receive,
            "remove" => StockMovementType.Remove,
            "adjust" => StockMovementType.Adjust,
            _ => throw new ArgumentException("Stock movement type must be receive, remove, or adjust.", nameof(value)),
        };
    }

    public static string ToDtoValue(StockMovementType type)
    {
        return type switch
        {
            StockMovementType.Receive => "receive",
            StockMovementType.Remove => "remove",
            StockMovementType.Adjust => "adjust",
            _ => throw new ArgumentException("Stock movement type is invalid.", nameof(type)),
        };
    }
}
