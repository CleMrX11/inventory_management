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

    protected override decimal Vat(SaleMode? saleMode)
    {
        if (!saleMode.HasValue)
        {
            throw new ArgumentException("Sale mode is required to calculate food tax.", nameof(saleMode));
        }

        if (!Enum.IsDefined(saleMode.Value))
        {
            throw new ArgumentException("Sale mode is invalid.", nameof(saleMode));
        }

        return saleMode == SaleMode.Takeaway
            ? 0.055m
            : 0.10m;
    }
}
