using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Tests.Domain;

public sealed class ArticleTests
{
    [Fact]
    public void CreateRejectsPriceIncludingTaxLowerThanPriceExcludingTax()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(120),
            new Money(100),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: PackagingLevel.New));
    }

    [Fact]
    public void CreateRejectsInvalidCategory()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            (ArticleCategory)999,
            new Money(100),
            new Money(120),
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
            new Money(4.4m),
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
            new Money(4.4m),
            DateOnly.FromDateTime(DateTime.Today),
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
            new Money(4.4m),
            DateOnly.FromDateTime(DateTime.Today),
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
            new Money(4.4m),
            DateOnly.FromDateTime(DateTime.Today),
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
            new Money(120),
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
            new Money(120),
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
            new Money(120),
            DateOnly.FromDateTime(DateTime.Today),
            TakeawayAvailability.Both,
            PackagingLevel.New));
    }
}
