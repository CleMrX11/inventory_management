namespace InventoryManagement.Api.Requests;

public sealed record CreateStockMovementRequest(
    string Type,
    int Quantity,
    string Reason,
    string? ExpirationDate,
    string? TakeawayAvailability,
    string? PackagingLevel);
