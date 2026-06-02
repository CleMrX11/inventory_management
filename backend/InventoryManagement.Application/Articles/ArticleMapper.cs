using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles;

internal static class ArticleMapper
{
    public static ArticleDto ToDto(Article article)
    {
        return new ArticleDto(
            article.Id.Value,
            article.Reference.Value,
            article.Name,
            article.Category.ToString(),
            article.PriceExcludingTax.Amount);
    }
}
