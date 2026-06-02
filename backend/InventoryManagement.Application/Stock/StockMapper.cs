using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Application.Stock;

internal static class StockMapper
{
    public static StockDto ToDto(Article article, IReadOnlyCollection<StockItem> stockItems)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var lots = stockItems
            .OrderBy(stockItem => stockItem.ExpirationDate)
            .ThenBy(stockItem => stockItem.PackagingLevel)
            .Select(stockItem => ToLotDto(article, stockItem, today))
            .ToList();
        var movements = stockItems
            .SelectMany(stockItem => stockItem.Movements.Select(movement => ToDto(stockItem, movement)))
            .OrderByDescending(movement => movement.OccurredAt)
            .ToList();

        return new StockDto(
            article.Id.Value,
            lots.Sum(lot => lot.CurrentQuantity),
            lots.Sum(lot => lot.SellableQuantity),
            lots.Sum(lot => lot.SellableValueIncludingTax),
            lots,
            movements);
    }

    public static StockMovementDto ToDto(StockItem stockItem, StockMovement movement)
    {
        return new StockMovementDto(
            movement.Id.Value,
            stockItem.Id.Value,
            stockItem.ArticleId.Value,
            StockMovementTypeParser.ToDtoValue(movement.Type),
            movement.Quantity,
            movement.QuantityBefore,
            movement.QuantityAfter,
            movement.Reason,
            movement.OccurredAt,
            stockItem.ExpirationDate?.ToString("yyyy-MM-dd"),
            stockItem.TakeawayAvailability?.ToString(),
            stockItem.PackagingLevel?.ToString());
    }

    private static StockLotDto ToLotDto(Article article, StockItem stockItem, DateOnly today)
    {
        var sellableQuantity = CalculateSellableQuantity(article, stockItem, today);
        var priceIncludingTax = article.CalculatePriceIncludingTax(stockItem.TakeawayAvailability);

        return new StockLotDto(
            stockItem.Id.Value,
            stockItem.ArticleId.Value,
            stockItem.CurrentQuantity,
            sellableQuantity,
            stockItem.ExpirationDate?.ToString("yyyy-MM-dd"),
            stockItem.TakeawayAvailability?.ToString(),
            stockItem.PackagingLevel?.ToString(),
            priceIncludingTax,
            sellableQuantity * priceIncludingTax);
    }

    private static int CalculateSellableQuantity(Article article, StockItem stockItem, DateOnly today)
    {
        return article.Category switch
        {
            ArticleCategory.Merchandise when stockItem.PackagingLevel == PackagingLevel.Unsellable => 0,
            ArticleCategory.FoodItem when stockItem.ExpirationDate < today => 0,
            _ => stockItem.CurrentQuantity,
        };
    }
}
