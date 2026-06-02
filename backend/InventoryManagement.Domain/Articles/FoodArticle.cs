namespace InventoryManagement.Domain.Articles;

public sealed class FoodArticle : Article
{
    public DateOnly ExpirationDate { get; private set; }
    public TakeawayAvailability TakeawayAvailability { get; private set; }

    private FoodArticle()
    {
    }

    internal FoodArticle(
        ArticleId id,
        Ean13Reference reference,
        string name,
        Money priceExcludingTax,
        DateOnly expirationDate,
        TakeawayAvailability takeawayAvailability,
        PackagingLevel? packagingLevel)
        : base(id, reference, name, ArticleCategory.FoodItem, priceExcludingTax)
    {
        if (packagingLevel.HasValue)
        {
            throw new ArgumentException("Packaging level is only valid for merchandise articles.", nameof(packagingLevel));
        }

        UpdateFoodDetails(expirationDate, takeawayAvailability);
        RecalculatePriceIncludingTax();
    }

    public void Update(
        string name,
        Money priceExcludingTax,
        DateOnly expirationDate,
        TakeawayAvailability takeawayAvailability)
    {
        UpdateCommon(name, priceExcludingTax);
        UpdateFoodDetails(expirationDate, takeawayAvailability);
        RecalculatePriceIncludingTax();
    }

    private void UpdateFoodDetails(DateOnly expirationDate, TakeawayAvailability takeawayAvailability)
    {
        if (!Enum.IsDefined(takeawayAvailability))
        {
            throw new ArgumentException("Takeaway availability is invalid.", nameof(takeawayAvailability));
        }

        ExpirationDate = expirationDate;
        TakeawayAvailability = takeawayAvailability;
    }

    protected override decimal Vax()
    {
        if (TakeawayAvailability == TakeawayAvailability.TakeawayOnly ||
            TakeawayAvailability == TakeawayAvailability.Both)
        {
            return 0.055M;
        }
        else
        {
            return 0.10M;
        }
    }
}
