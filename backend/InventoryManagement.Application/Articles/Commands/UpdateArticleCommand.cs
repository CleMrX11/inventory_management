namespace InventoryManagement.Application.Articles.Commands;

public sealed record UpdateArticleCommand(
    Guid Id,
    string Reference,
    string Name,
    decimal PriceExcludingTax,
    decimal PriceIncludingTax);
