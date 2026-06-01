using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Articles;

public sealed class Article : AggregateRoot<ArticleId>
{
    public Ean13Reference Reference { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Money PriceExcludingTax { get; private set; }
    public Money PriceIncludingTax { get; private set; }

    private Article()
    {
    }

    private Article(
        ArticleId id,
        Ean13Reference reference,
        string name,
        Money priceExcludingTax,
        Money priceIncludingTax) : base(id)
    {
        Reference = reference;
        Update(name, priceExcludingTax, priceIncludingTax);
    }

    public static Article Create(
        Ean13Reference reference,
        string name,
        Money priceExcludingTax,
        Money priceIncludingTax)
    {
        return new Article(ArticleId.New(), reference, name, priceExcludingTax, priceIncludingTax);
    }

    public void Update(string name, Money priceExcludingTax, Money priceIncludingTax)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Article name is required.", nameof(name));
        }

        if (priceIncludingTax.Amount < priceExcludingTax.Amount)
        {
            throw new ArgumentException("Price including tax must be greater than or equal to price excluding tax.");
        }

        Name = name.Trim();
        PriceExcludingTax = priceExcludingTax;
        PriceIncludingTax = priceIncludingTax;
    }

    public void ChangeReference(Ean13Reference reference)
    {
        Reference = reference;
    }
}
