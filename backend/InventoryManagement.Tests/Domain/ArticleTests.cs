using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Tests.Domain;

public sealed class ArticleTests
{
    [Fact]
    public void CreateFoodArticleWithTakeawayOnlyAppliesFivePercentTax()
    {
        var article = Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4),
            expirationDate: new DateOnly(2026, 6, 1),
            takeawayAvailability: TakeawayAvailability.TakeawayOnly,
            packagingLevel: null);

        Assert.Equal(new Money(4), article.PriceExcludingTax);
        Assert.Equal(new Money(4.2m), article.PriceIncludingTax);
    }

    [Fact]
    public void CreateFoodArticleWithBothAppliesFivePercentTax()
    {
        var article = Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4),
            expirationDate: new DateOnly(2026, 6, 1),
            takeawayAvailability: TakeawayAvailability.Both,
            packagingLevel: null);

        Assert.Equal(new Money(4), article.PriceExcludingTax);
        Assert.Equal(new Money(4.2m), article.PriceIncludingTax);
    }

    [Fact]
    public void CreateFoodArticleWithOnSiteOnlyAppliesTenPercentTax()
    {
        var article = Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4),
            expirationDate: new DateOnly(2026, 6, 1),
            takeawayAvailability: TakeawayAvailability.OnSiteOnly,
            packagingLevel: null);

        Assert.Equal(new Money(4), article.PriceExcludingTax);
        Assert.Equal(new Money(4.4m), article.PriceIncludingTax);
    }

    [Fact]
    public void CreateMerchandiseArticleAppliesTwentyPercentTax()
    {
        var article = Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: PackagingLevel.New);

        Assert.Equal(new Money(100), article.PriceExcludingTax);
        Assert.Equal(new Money(120), article.PriceIncludingTax);
    }

    [Fact]
    public void CreateRejectsInvalidCategory()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            (ArticleCategory)999,
            new Money(100),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: PackagingLevel.New));
    }

    [Fact]
    public void CreateFoodArticleRejectsMissingExpirationDate()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4),
            expirationDate: null,
            takeawayAvailability: TakeawayAvailability.Both,
            packagingLevel: null));
    }

    [Fact]
    public void CreateFoodArticleRejectsMissingTakeawayAvailability()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4),
            new DateOnly(2026, 6, 1),
            takeawayAvailability: null,
            packagingLevel: null));
    }

    [Fact]
    public void CreateFoodArticleRejectsPackagingLevel()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4),
            new DateOnly(2026, 6, 1),
            TakeawayAvailability.Both,
            PackagingLevel.New));
    }

    [Fact]
    public void CreateFoodArticleRejectsInvalidTakeawayAvailability()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4),
            new DateOnly(2026, 6, 1),
            (TakeawayAvailability)999,
            packagingLevel: null));
    }

    [Fact]
    public void CreateMerchandiseArticleRejectsMissingPackagingLevel()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: null));
    }

    [Fact]
    public void CreateMerchandiseArticleRejectsInvalidPackagingLevel()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: (PackagingLevel)999));
    }

    [Fact]
    public void CreateMerchandiseArticleRejectsFoodSpecificFields()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100),
            new DateOnly(2026, 6, 1),
            TakeawayAvailability.Both,
            PackagingLevel.New));
    }
}
