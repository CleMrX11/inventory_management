using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Articles;

public abstract class Article : AggregateRoot<ArticleId>
{
    public Ean13Reference Reference { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ArticleCategory Category { get; private set; }
    public Money PriceExcludingTax { get; private set; }
    public Money PriceIncludingTax { get; private set; }

    protected Article()
    {
    }

    protected Article(
        ArticleId id,
        Ean13Reference reference,
        string name,
        ArticleCategory category,
        Money priceExcludingTax) : base(id)
    {
        Reference = reference;
        Update(name, category, priceExcludingTax);
    }

    public static Article Create(
        Ean13Reference reference,
        string name,
        ArticleCategory category,
        Money priceExcludingTax,
        DateOnly? expirationDate,
        TakeawayAvailability? takeawayAvailability,
        PackagingLevel? packagingLevel)
    {
        return Create(
            ArticleId.New(),
            reference,
            name,
            category,
            priceExcludingTax,
            expirationDate,
            takeawayAvailability,
            packagingLevel);
    }

    public static Article Create(
        ArticleId id,
        Ean13Reference reference,
        string name,
        ArticleCategory category,
        Money priceExcludingTax,
        DateOnly? expirationDate,
        TakeawayAvailability? takeawayAvailability,
        PackagingLevel? packagingLevel)
    {
        return category switch
        {
            ArticleCategory.FoodItem => new FoodArticle(
                id,
                reference,
                name,
                priceExcludingTax,
                expirationDate ?? throw new ArgumentException("Expiration date is required for food articles.", nameof(expirationDate)),
                takeawayAvailability ?? throw new ArgumentException("Takeaway availability is required for food articles.", nameof(takeawayAvailability)),
                packagingLevel),
            ArticleCategory.Merchandise => new MerchandiseArticle(
                id,
                reference,
                name,
                priceExcludingTax,
                packagingLevel ?? throw new ArgumentException("Packaging level is required for merchandise articles.", nameof(packagingLevel)),
                expirationDate,
                takeawayAvailability),
            _ => throw new ArgumentException("Article category is invalid.", nameof(category)),
        };
    }

    public void UpdateCommon(string name, Money priceExcludingTax)
    {
        Update(name, Category, priceExcludingTax);
    }

    private void Update(string name, ArticleCategory category, Money priceExcludingTax)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Article name is required.", nameof(name));
        }

        if (!Enum.IsDefined(category))
        {
            throw new ArgumentException("Article category is invalid.", nameof(category));
        }

        Name = name.Trim();
        Category = category;
        PriceExcludingTax = priceExcludingTax;
    }

    public void ChangeReference(Ean13Reference reference)
    {
        Reference = reference;
    }

    protected abstract decimal Vax();

    protected void RecalculatePriceIncludingTax()
    {
        PriceIncludingTax = new Money(PriceExcludingTax.Amount + (Vax() * PriceExcludingTax.Amount));
    }
}
