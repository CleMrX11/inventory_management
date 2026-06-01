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
        Money priceExcludingTax,
        Money priceIncludingTax) : base(id)
    {
        Reference = reference;
        Update(name, category, priceExcludingTax, priceIncludingTax);
    }

    public static Article Create(
        Ean13Reference reference,
        string name,
        ArticleCategory category,
        Money priceExcludingTax,
        Money priceIncludingTax,
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
            priceIncludingTax,
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
        Money priceIncludingTax,
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
                priceIncludingTax,
                expirationDate ?? throw new ArgumentException("Expiration date is required for food articles.", nameof(expirationDate)),
                takeawayAvailability ?? throw new ArgumentException("Takeaway availability is required for food articles.", nameof(takeawayAvailability)),
                packagingLevel),
            ArticleCategory.Merchandise => new MerchandiseArticle(
                id,
                reference,
                name,
                priceExcludingTax,
                priceIncludingTax,
                packagingLevel ?? throw new ArgumentException("Packaging level is required for merchandise articles.", nameof(packagingLevel)),
                expirationDate,
                takeawayAvailability),
            _ => throw new ArgumentException("Article category is invalid.", nameof(category)),
        };
    }

    public void UpdateCommon(string name, Money priceExcludingTax, Money priceIncludingTax)
    {
        Update(name, Category, priceExcludingTax, priceIncludingTax);
    }

    private void Update(string name, ArticleCategory category, Money priceExcludingTax, Money priceIncludingTax)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Article name is required.", nameof(name));
        }

        if (!Enum.IsDefined(category))
        {
            throw new ArgumentException("Article category is invalid.", nameof(category));
        }

        if (priceIncludingTax.Amount < priceExcludingTax.Amount)
        {
            throw new ArgumentException("Price including tax must be greater than or equal to price excluding tax.");
        }

        Name = name.Trim();
        Category = category;
        PriceExcludingTax = priceExcludingTax;
        PriceIncludingTax = priceIncludingTax;
    }

    public void ChangeReference(Ean13Reference reference)
    {
        Reference = reference;
    }
}
