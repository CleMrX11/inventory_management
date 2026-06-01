namespace InventoryManagement.Domain.Stock;

public readonly record struct StockItemId(Guid Value)
{
    public static StockItemId New() => new(Guid.NewGuid());
}
