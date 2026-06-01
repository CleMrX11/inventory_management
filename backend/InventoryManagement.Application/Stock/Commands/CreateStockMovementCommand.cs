namespace InventoryManagement.Application.Stock.Commands;

public sealed record CreateStockMovementCommand(
    Guid ArticleId,
    string Type,
    int Quantity,
    string Reason);
