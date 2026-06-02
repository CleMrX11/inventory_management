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

    protected override decimal Vax(TakeawayAvailability? takeawayAvailability)
    {
        if (!takeawayAvailability.HasValue)
        {
            throw new ArgumentException("Takeaway availability is required to calculate food tax.", nameof(takeawayAvailability));
        }

        if (!Enum.IsDefined(takeawayAvailability.Value))
        {
            throw new ArgumentException("Takeaway availability is invalid.", nameof(takeawayAvailability));
        }

        return takeawayAvailability is TakeawayAvailability.TakeawayOnly or TakeawayAvailability.Both
            ? 0.055m
            : 0.10m;
    }
}
