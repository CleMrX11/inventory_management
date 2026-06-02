namespace InventoryManagement.Application.Stock;

public sealed record StockDto(
    Guid ArticleId,
    int CurrentQuantity,
    int SellableQuantity,
    IReadOnlyList<StockLotDto> Lots,
    IReadOnlyList<StockMovementDto> Movements);

public sealed record StockLotDto(
    Guid Id,
    Guid ArticleId,
    int CurrentQuantity,
    int SellableQuantity,
    string? ExpirationDate,
    string? TakeawayAvailability,
    string? PackagingLevel,
    decimal PriceIncludingTax);
