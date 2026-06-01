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
        Money priceIncludingTax,
        DateOnly expirationDate,
        TakeawayAvailability takeawayAvailability,
        PackagingLevel? packagingLevel)
        : base(id, reference, name, ArticleCategory.FoodItem, priceExcludingTax, priceIncludingTax)
    {
        if (packagingLevel.HasValue)
        {
            throw new ArgumentException("Packaging level is only valid for merchandise articles.", nameof(packagingLevel));
        }

        UpdateFoodDetails(expirationDate, takeawayAvailability);
    }

    public void Update(
        string name,
        Money priceExcludingTax,
        Money priceIncludingTax,
        DateOnly expirationDate,
        TakeawayAvailability takeawayAvailability)
    {
        UpdateCommon(name, priceExcludingTax, priceIncludingTax);
        UpdateFoodDetails(expirationDate, takeawayAvailability);
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
}
