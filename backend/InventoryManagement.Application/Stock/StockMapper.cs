using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Application.Stock;

internal static class StockMapper
{
    public static StockDto ToDto(Article article, StockItem? stockItem)
    {
        var currentQuantity = stockItem?.CurrentQuantity ?? 0;
        var sellableQuantity = CalculateSellableQuantity(article, currentQuantity);
        var movements = stockItem?.Movements
            .OrderByDescending(movement => movement.OccurredAt)
            .Select(movement => ToDto(stockItem, movement))
            .ToList() ?? [];

        return new StockDto(article.Id.Value, currentQuantity, sellableQuantity, movements);
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

    private static int CalculateSellableQuantity(Article article, int currentQuantity)
    {
        if (article is MerchandiseArticle { PackagingLevel: PackagingLevel.Unsellable })
        {
            return 0;
        }

        return currentQuantity;
    }
}
