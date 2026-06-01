namespace InventoryManagement.Api.Requests;

public sealed record CreateArticleRequest(
    string Reference,
    string Name,
    decimal PriceExcludingTax,
    decimal PriceIncludingTax);
