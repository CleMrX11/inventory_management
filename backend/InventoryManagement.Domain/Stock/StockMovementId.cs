namespace InventoryManagement.Domain.Stock;

public readonly record struct StockMovementId(Guid Value)
{
    public static StockMovementId New() => new(Guid.NewGuid());
}
