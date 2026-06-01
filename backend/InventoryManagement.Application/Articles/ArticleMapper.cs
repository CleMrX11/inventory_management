using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles;

internal static class ArticleMapper
{
    public static ArticleDto ToDto(Article article)
    {
        var foodArticle = article as FoodArticle;
        var merchandiseArticle = article as MerchandiseArticle;

        return new ArticleDto(
            article.Id.Value,
            article.Reference.Value,
            article.Name,
            article.Category.ToString(),
            article.PriceExcludingTax.Amount,
            article.PriceIncludingTax.Amount,
            foodArticle?.ExpirationDate.ToString("yyyy-MM-dd"),
            foodArticle?.TakeawayAvailability.ToString(),
            merchandiseArticle?.PackagingLevel.ToString());
    }
}
