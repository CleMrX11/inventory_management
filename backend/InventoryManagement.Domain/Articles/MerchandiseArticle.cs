namespace InventoryManagement.Domain.Articles;

public sealed class MerchandiseArticle : Article
{
    public PackagingLevel PackagingLevel { get; private set; }

    private MerchandiseArticle()
    {
    }

    internal MerchandiseArticle(
        ArticleId id,
        Ean13Reference reference,
        string name,
        Money priceExcludingTax,
        PackagingLevel packagingLevel,
        DateOnly? expirationDate,
        TakeawayAvailability? takeawayAvailability)
        : base(id, reference, name, ArticleCategory.Merchandise, priceExcludingTax)
    {
        if (expirationDate.HasValue)
        {
            throw new ArgumentException("Expiration date is only valid for food articles.", nameof(expirationDate));
        }

        if (takeawayAvailability.HasValue)
        {
            throw new ArgumentException("Takeaway availability is only valid for food articles.", nameof(takeawayAvailability));
        }

        UpdateMerchandiseDetails(packagingLevel);
        RecalculatePriceIncludingTax();
    }

    public void Update(
        string name,
        Money priceExcludingTax,
        PackagingLevel packagingLevel)
    {
        UpdateCommon(name, priceExcludingTax);
        UpdateMerchandiseDetails(packagingLevel);
        RecalculatePriceIncludingTax();
    }

    private void UpdateMerchandiseDetails(PackagingLevel packagingLevel)
    {
        if (!Enum.IsDefined(packagingLevel))
        {
            throw new ArgumentException("Packaging level is invalid.", nameof(packagingLevel));
        }

        PackagingLevel = packagingLevel;
    }

    protected override decimal Vax()
    {
        return 0.2M;
    }
}
