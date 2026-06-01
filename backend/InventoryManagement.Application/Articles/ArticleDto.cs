namespace InventoryManagement.Application.Articles;

public sealed record ArticleDto(
    Guid Id,
    string Reference,
    string Name,
    decimal PriceExcludingTax,
    decimal PriceIncludingTax);
