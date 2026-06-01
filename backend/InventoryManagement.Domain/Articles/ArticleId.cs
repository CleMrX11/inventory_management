namespace InventoryManagement.Domain.Articles;

public readonly record struct ArticleId(Guid Value)
{
    public static ArticleId New() => new(Guid.NewGuid());
}
