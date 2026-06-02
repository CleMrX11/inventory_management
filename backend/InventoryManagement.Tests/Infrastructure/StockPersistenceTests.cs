using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Tests.Infrastructure;

public sealed class StockPersistenceTests
{
    [Fact]
    public async Task SavesStockItemAndMovementHistory()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var article = CreateMerchandiseArticle();

        await using (var dbContext = new AppDbContext(options))
        {
            await dbContext.Database.EnsureCreatedAsync();

            var newStockItem = StockItem.Create(article, null, null, PackagingLevel.New);
            newStockItem.Receive(10, "Supplier delivery");
            newStockItem.Remove(3, "Damaged");
            var unsellableStockItem = StockItem.Create(article, null, null, PackagingLevel.Unsellable);
            unsellableStockItem.Receive(2, "Damaged return");

            dbContext.Articles.Add(article);
            dbContext.StockItems.AddRange(newStockItem, unsellableStockItem);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AppDbContext(options))
        {
            var persistedStockItems = await dbContext.StockItems
                .Include(stockItem => stockItem.Movements)
                .Where(stockItem => stockItem.ArticleId == article.Id)
                .ToListAsync();
            var persistedStockItem = persistedStockItems.Single(stockItem => stockItem.PackagingLevel == PackagingLevel.New);

            Assert.Equal(7, persistedStockItem.CurrentQuantity);
            Assert.Equal(2, persistedStockItems.Single(stockItem => stockItem.PackagingLevel == PackagingLevel.Unsellable).CurrentQuantity);
            Assert.Collection(
                persistedStockItem.Movements.OrderBy(movement => movement.QuantityBefore),
                movement =>
                {
                    Assert.Equal(StockMovementType.Receive, movement.Type);
                    Assert.Equal(0, movement.QuantityBefore);
                    Assert.Equal(10, movement.QuantityAfter);
                },
                movement =>
                {
                    Assert.Equal(StockMovementType.Remove, movement.Type);
                    Assert.Equal(10, movement.QuantityBefore);
                    Assert.Equal(7, movement.QuantityAfter);
                });
        }
    }

    private static Article CreateMerchandiseArticle()
    {
        return Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100));
    }
}
