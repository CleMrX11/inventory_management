using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Tests.Domain;

public sealed class StockItemTests
{
    [Fact]
    public void ReceiveIncreasesCurrentQuantityAndRecordsMovement()
    {
        var stockItem = CreateMerchandiseStockItem();

        var movement = stockItem.Receive(10, " Supplier delivery ");

        Assert.Equal(10, stockItem.CurrentQuantity);
        Assert.Equal(StockMovementType.Receive, movement.Type);
        Assert.Equal(10, movement.Quantity);
        Assert.Equal(0, movement.QuantityBefore);
        Assert.Equal(10, movement.QuantityAfter);
        Assert.Equal("Supplier delivery", movement.Reason);
        Assert.Single(stockItem.Movements);
    }

    [Fact]
    public void RemoveDecreasesCurrentQuantityAndRecordsMovement()
    {
        var stockItem = CreateMerchandiseStockItem();
        stockItem.Receive(10, "Supplier delivery");

        var movement = stockItem.Remove(4, "Damaged");

        Assert.Equal(6, stockItem.CurrentQuantity);
        Assert.Equal(StockMovementType.Remove, movement.Type);
        Assert.Equal(4, movement.Quantity);
        Assert.Equal(10, movement.QuantityBefore);
        Assert.Equal(6, movement.QuantityAfter);
    }

    [Fact]
    public void AdjustSetsCurrentQuantityAndRecordsMovement()
    {
        var stockItem = CreateMerchandiseStockItem();
        stockItem.Receive(10, "Supplier delivery");

        var movement = stockItem.AdjustTo(3, "Inventory count");

        Assert.Equal(3, stockItem.CurrentQuantity);
        Assert.Equal(StockMovementType.Adjust, movement.Type);
        Assert.Equal(3, movement.Quantity);
        Assert.Equal(10, movement.QuantityBefore);
        Assert.Equal(3, movement.QuantityAfter);
    }

    [Fact]
    public void RemoveRejectsQuantityGreaterThanAvailable()
    {
        var stockItem = CreateMerchandiseStockItem();

        Assert.Throws<InvalidOperationException>(() => stockItem.Remove(1, "Sale"));
    }

    [Fact]
    public void AdjustRejectsNegativeQuantity()
    {
        var stockItem = CreateMerchandiseStockItem();

        Assert.Throws<ArgumentException>(() => stockItem.AdjustTo(-1, "Inventory count"));
    }

    [Fact]
    public void ReceiveRejectsNonPositiveQuantity()
    {
        var stockItem = CreateMerchandiseStockItem();

        Assert.Throws<ArgumentException>(() => stockItem.Receive(0, "Supplier delivery"));
    }

    [Fact]
    public void CreateFoodLotRejectsMissingExpirationDate()
    {
        var article = CreateFoodArticle();

        Assert.Throws<ArgumentException>(() => StockItem.Create(
            article,
            expirationDate: null,
            takeawayAvailability: TakeawayAvailability.Both,
            packagingLevel: null));
    }

    [Fact]
    public void CreateFoodLotRejectsPackagingLevel()
    {
        var article = CreateFoodArticle();

        Assert.Throws<ArgumentException>(() => StockItem.Create(
            article,
            new DateOnly(2026, 6, 30),
            TakeawayAvailability.Both,
            PackagingLevel.New));
    }

    [Fact]
    public void CreateMerchandiseLotRejectsMissingPackagingLevel()
    {
        var article = CreateMerchandiseArticle();

        Assert.Throws<ArgumentException>(() => StockItem.Create(
            article,
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: null));
    }

    private static StockItem CreateMerchandiseStockItem()
    {
        return StockItem.Create(
            CreateMerchandiseArticle(),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: PackagingLevel.New);
    }

    private static Article CreateMerchandiseArticle()
    {
        return Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100));
    }

    private static Article CreateFoodArticle()
    {
        return Article.Create(
            new Ean13Reference("5901234123457"),
            "Sandwich",
            ArticleCategory.FoodItem,
            new Money(4));
    }
}
