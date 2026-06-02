namespace InventoryManagement.Domain.Articles;

public sealed class FoodArticle : Article
{
    private FoodArticle()
    {
    }

    internal FoodArticle(
        ArticleId id,
        Ean13Reference reference,
        string name,
        Money priceExcludingTax)
        : base(id, reference, name, ArticleCategory.FoodItem, priceExcludingTax)
    {
    }

    public void Update(
        string name,
        Money priceExcludingTax)
    {
        UpdateCommon(name, priceExcludingTax);
    }
}
