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

            var stockItem = StockItem.Create(article.Id);
            stockItem.Receive(10, "Supplier delivery");
            stockItem.Remove(3, "Damaged");

            dbContext.Articles.Add(article);
            dbContext.StockItems.Add(stockItem);
            await dbContext.SaveChangesAsync();
        }

        await using (var dbContext = new AppDbContext(options))
        {
            var persistedStockItem = await dbContext.StockItems
                .Include(stockItem => stockItem.Movements)
                .SingleAsync(stockItem => stockItem.ArticleId == article.Id);

            Assert.Equal(7, persistedStockItem.CurrentQuantity);
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
            new Money(100),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: PackagingLevel.New);
    }
}
