using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles;

internal static class ArticleCategoryParser
{
    public static ArticleCategory Parse(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return ArticleCategory.Merchandise;
        }

        return Enum.TryParse<ArticleCategory>(category, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed)
            ? parsed
            : throw new ArgumentException("Article category is invalid.", nameof(category));
    }
}
