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
        Money priceIncludingTax,
        PackagingLevel packagingLevel,
        DateOnly? expirationDate,
        TakeawayAvailability? takeawayAvailability)
        : base(id, reference, name, ArticleCategory.Merchandise, priceExcludingTax, priceIncludingTax)
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
    }

    public void Update(
        string name,
        Money priceExcludingTax,
        Money priceIncludingTax,
        PackagingLevel packagingLevel)
    {
        UpdateCommon(name, priceExcludingTax, priceIncludingTax);
        UpdateMerchandiseDetails(packagingLevel);
    }

    private void UpdateMerchandiseDetails(PackagingLevel packagingLevel)
    {
        if (!Enum.IsDefined(packagingLevel))
        {
            throw new ArgumentException("Packaging level is invalid.", nameof(packagingLevel));
        }

        PackagingLevel = packagingLevel;
    }
}
