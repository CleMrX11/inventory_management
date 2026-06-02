using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Articles;

public abstract class Article : AggregateRoot<ArticleId>
{
    public Ean13Reference Reference { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ArticleCategory Category { get; private set; }
    public Money PriceExcludingTax { get; private set; }

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
        Money priceExcludingTax)
    {
        return Create(
            ArticleId.New(),
            reference,
            name,
            category,
            priceExcludingTax);
    }

    public static Article Create(
        ArticleId id,
        Ean13Reference reference,
        string name,
        ArticleCategory category,
        Money priceExcludingTax)
    {
        return category switch
        {
            ArticleCategory.FoodItem => new FoodArticle(
                id,
                reference,
                name,
                priceExcludingTax),
            ArticleCategory.Merchandise => new MerchandiseArticle(
                id,
                reference,
                name,
                priceExcludingTax),
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

    public decimal CalculatePriceIncludingTax(TakeawayAvailability? takeawayAvailability = null)
    {
        var taxRate = Category switch
        {
            ArticleCategory.FoodItem => takeawayAvailability is TakeawayAvailability.TakeawayOnly or TakeawayAvailability.Both
                ? 0.055m
                : 0.10m,
            ArticleCategory.Merchandise => 0.20m,
            _ => throw new InvalidOperationException("Article category is invalid."),
        };

        return PriceExcludingTax.Amount + (taxRate * PriceExcludingTax.Amount);
    }
}
