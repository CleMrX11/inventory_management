namespace InventoryManagement.Api.Requests;

public sealed record UpdateArticleRequest(
    string Reference,
    string Name,
    decimal PriceExcludingTax,
    decimal PriceIncludingTax);
