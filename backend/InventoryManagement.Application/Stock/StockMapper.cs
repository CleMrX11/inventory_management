using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Application.Stock;

internal static class StockMapper
{
    public static StockDto ToDto(StockItem stockItem)
    {
        return new StockDto(stockItem.ArticleId.Value, stockItem.CurrentQuantity);
    }

    public static StockMovementDto ToDto(StockItem stockItem, StockMovement movement)
    {
        return new StockMovementDto(
            movement.Id.Value,
            stockItem.ArticleId.Value,
            StockMovementTypeParser.ToDtoValue(movement.Type),
            movement.Quantity,
            movement.QuantityBefore,
            movement.QuantityAfter,
            movement.Reason,
            movement.OccurredAt);
    }
}
