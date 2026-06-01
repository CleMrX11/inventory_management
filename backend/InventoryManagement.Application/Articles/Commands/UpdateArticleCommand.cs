namespace InventoryManagement.Application.Articles.Commands;

public sealed record UpdateArticleCommand(
    Guid Id,
    string Reference,
    string Name,
    string Category,
    decimal PriceExcludingTax,
    decimal PriceIncludingTax,
    string? ExpirationDate,
    string? TakeawayAvailability,
    string? PackagingLevel);
