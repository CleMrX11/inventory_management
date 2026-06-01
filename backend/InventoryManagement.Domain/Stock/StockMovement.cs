using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Stock;

public sealed class StockMovement : Entity<StockMovementId>
{
    public StockItemId StockItemId { get; private set; }
    public StockMovementType Type { get; private set; }
    public int Quantity { get; private set; }
    public int QuantityBefore { get; private set; }
    public int QuantityAfter { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; private set; }

    private StockMovement()
    {
    }

    internal StockMovement(
        StockMovementId id,
        StockItemId stockItemId,
        StockMovementType type,
        int quantity,
        int quantityBefore,
        int quantityAfter,
        string reason,
        DateTimeOffset occurredAt) : base(id)
    {
        StockItemId = stockItemId;
        Type = type;
        Quantity = quantity;
        QuantityBefore = quantityBefore;
        QuantityAfter = quantityAfter;
        Reason = reason;
        OccurredAt = occurredAt;
    }
}
