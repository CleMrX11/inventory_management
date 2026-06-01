using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Tests.Domain;

public sealed class StockItemTests
{
    [Fact]
    public void ReceiveIncreasesCurrentQuantityAndRecordsMovement()
    {
        var stockItem = StockItem.Create(ArticleId.New());

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
        var stockItem = StockItem.Create(ArticleId.New());
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
        var stockItem = StockItem.Create(ArticleId.New());
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
        var stockItem = StockItem.Create(ArticleId.New());

        Assert.Throws<InvalidOperationException>(() => stockItem.Remove(1, "Sale"));
    }

    [Fact]
    public void AdjustRejectsNegativeQuantity()
    {
        var stockItem = StockItem.Create(ArticleId.New());

        Assert.Throws<ArgumentException>(() => stockItem.AdjustTo(-1, "Inventory count"));
    }

    [Fact]
    public void ReceiveRejectsNonPositiveQuantity()
    {
        var stockItem = StockItem.Create(ArticleId.New());

        Assert.Throws<ArgumentException>(() => stockItem.Receive(0, "Supplier delivery"));
    }
}
