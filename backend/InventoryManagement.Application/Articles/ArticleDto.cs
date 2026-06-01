namespace InventoryManagement.Application.Articles;

public sealed record ArticleDto(
    Guid Id,
    string Reference,
    string Name,
    string Category,
    decimal PriceExcludingTax,
    decimal PriceIncludingTax,
    string? ExpirationDate,
    string? TakeawayAvailability,
    string? PackagingLevel);
