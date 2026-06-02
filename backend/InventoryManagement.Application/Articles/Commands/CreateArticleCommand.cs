namespace InventoryManagement.Application.Articles.Commands;

public sealed record CreateArticleCommand(
    string Reference,
    string Name,
    string Category,
    decimal PriceExcludingTax);
