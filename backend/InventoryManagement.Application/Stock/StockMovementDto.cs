namespace InventoryManagement.Application.Stock;

public sealed record StockMovementDto(
    Guid Id,
    Guid ArticleId,
    string Type,
    int Quantity,
    int QuantityBefore,
    int QuantityAfter,
    string Reason,
    DateTimeOffset OccurredAt);
