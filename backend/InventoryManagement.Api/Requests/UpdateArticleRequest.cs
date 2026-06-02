namespace InventoryManagement.Api.Requests;

public sealed record UpdateArticleRequest(
    string Reference,
    string Name,
    string Category,
    decimal PriceExcludingTax,
    string? ExpirationDate,
    string? TakeawayAvailability,
    string? PackagingLevel);
