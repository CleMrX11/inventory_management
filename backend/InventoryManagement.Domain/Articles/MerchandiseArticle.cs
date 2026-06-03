namespace InventoryManagement.Domain.Articles;

public sealed class MerchandiseArticle : Article
{
    private MerchandiseArticle()
    {
    }

    internal MerchandiseArticle(
        ArticleId id,
        Ean13Reference reference,
        string name,
        Money priceExcludingTax)
        : base(id, reference, name, ArticleCategory.Merchandise, priceExcludingTax)
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
        return 0.20m;
    }
}
