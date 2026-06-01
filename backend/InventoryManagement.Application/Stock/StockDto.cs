namespace InventoryManagement.Application.Stock;

public sealed record StockDto(
    Guid ArticleId,
    int CurrentQuantity);
