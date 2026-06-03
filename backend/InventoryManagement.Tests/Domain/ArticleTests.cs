using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Tests.Domain;

public sealed class ArticleTests
{
    [Fact]
    public void CalculatePriceIncludingTaxUsesFoodTakeawayTax()
    {
        var article = CreateFoodArticle();

        var priceIncludingTax = article.CalculatePriceIncludingTax(SaleMode.Takeaway);

        Assert.Equal(4.22m, priceIncludingTax);
    }

    [Fact]
    public void CalculatePriceIncludingTaxUsesFoodOnSiteTax()
    {
        var article = CreateFoodArticle();

        var priceIncludingTax = article.CalculatePriceIncludingTax(SaleMode.OnSite);

        Assert.Equal(4.40m, priceIncludingTax);
    }

    [Fact]
    public void CalculatePriceIncludingTaxRejectsMissingFoodTakeawayAvailability()
    {
        var article = CreateFoodArticle();

        Assert.Throws<ArgumentException>(() => article.CalculatePriceIncludingTax());
    }

    [Fact]
    public void CalculatePriceIncludingTaxRejectsInvalidFoodSaleMode()
    {
        var article = CreateFoodArticle();

        Assert.Throws<ArgumentException>(() => article.CalculatePriceIncludingTax((SaleMode)999));
    }

    [Fact]
    public void CalculatePriceIncludingTaxUsesMerchandiseTax()
    {
        var article = Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100));

        var priceIncludingTax = article.CalculatePriceIncludingTax();

        Assert.Equal(120, priceIncludingTax);
    }

    [Fact]
    public void CreateRejectsInvalidCategory()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            (ArticleCategory)999,
            new Money(100)));
    }

    private static Article CreateFoodArticle()
    {
        return Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4));
    }
}
