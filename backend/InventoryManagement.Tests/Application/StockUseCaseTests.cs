using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.UseCases;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.Stock.Commands;
using InventoryManagement.Application.Stock.UseCases;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Tests.Application;

public sealed class StockUseCaseTests
{
    [Fact]
    public async Task GetStockReturnsZeroWhenArticleHasNoStockLots()
    {
        var article = CreateMerchandiseArticle();
        var useCase = new GetStockByArticleIdUseCase(
            new InMemoryArticleRepository(article),
            new InMemoryStockItemRepository());

        var stock = await useCase.ExecuteAsync(article.Id.Value, CancellationToken.None);

        Assert.Equal(article.Id.Value, stock.ArticleId);
        Assert.Equal(0, stock.CurrentQuantity);
        Assert.Equal(0, stock.SellableQuantity);
        Assert.Empty(stock.Lots);
        Assert.Empty(stock.Movements);
    }

    [Fact]
    public async Task GetStockAggregatesMultipleMerchandiseLots()
    {
        var article = CreateMerchandiseArticle();
        var newLot = CreateMerchandiseStockItem(article, PackagingLevel.New);
        newLot.Receive(10, "Supplier delivery");
        var unsellableLot = CreateMerchandiseStockItem(article, PackagingLevel.Unsellable);
        unsellableLot.Receive(3, "Damaged return");
        var useCase = new GetStockByArticleIdUseCase(
            new InMemoryArticleRepository(article),
            new InMemoryStockItemRepository(newLot, unsellableLot));

        var stock = await useCase.ExecuteAsync(article.Id.Value, CancellationToken.None);

        Assert.Equal(13, stock.CurrentQuantity);
        Assert.Equal(10, stock.SellableQuantity);
        Assert.Equal(2, stock.Lots.Count);
        Assert.All(stock.Lots, lot => Assert.Equal(120, lot.PriceIncludingTax));
    }

    [Fact]
    public async Task GetStockExcludesExpiredFoodLotsFromSellableQuantity()
    {
        var article = CreateFoodArticle();
        var expiredLot = CreateFoodStockItem(article, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)));
        expiredLot.Receive(5, "Old delivery");
        var validLot = CreateFoodStockItem(article, DateOnly.FromDateTime(DateTime.Today.AddDays(1)));
        validLot.Receive(7, "Fresh delivery");
        var useCase = new GetStockByArticleIdUseCase(
            new InMemoryArticleRepository(article),
            new InMemoryStockItemRepository(expiredLot, validLot));

        var stock = await useCase.ExecuteAsync(article.Id.Value, CancellationToken.None);

        Assert.Equal(12, stock.CurrentQuantity);
        Assert.Equal(7, stock.SellableQuantity);
        Assert.Contains(stock.Lots, lot => lot.ExpirationDate == expiredLot.ExpirationDate?.ToString("yyyy-MM-dd") && lot.SellableQuantity == 0);
        Assert.Contains(stock.Lots, lot => lot.PriceIncludingTax == 4.22m);
    }

    [Fact]
    public async Task CreateStockMovementCreatesLotAndPersistsMovement()
    {
        var article = CreateMerchandiseArticle();
        var stockItems = new InMemoryStockItemRepository();
        var unitOfWork = new CountingUnitOfWork();
        var useCase = new CreateStockMovementUseCase(new InMemoryArticleRepository(article), stockItems, unitOfWork);

        var movement = await useCase.ExecuteAsync(
            new CreateStockMovementCommand(article.Id.Value, "receive", 10, "Supplier delivery", null, null, "New"),
            CancellationToken.None);

        var persistedLots = await stockItems.ListByArticleIdAsync(article.Id, CancellationToken.None);
        Assert.Single(persistedLots);
        Assert.Equal(10, persistedLots[0].CurrentQuantity);
        Assert.Equal("receive", movement.Type);
        Assert.Equal("New", movement.PackagingLevel);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateStockMovementUpdatesMatchingLot()
    {
        var article = CreateMerchandiseArticle();
        var existingLot = CreateMerchandiseStockItem(article, PackagingLevel.Refurbished);
        existingLot.Receive(2, "Initial");
        var stockItems = new InMemoryStockItemRepository(existingLot);
        var useCase = new CreateStockMovementUseCase(
            new InMemoryArticleRepository(article),
            stockItems,
            new CountingUnitOfWork());

        await useCase.ExecuteAsync(
            new CreateStockMovementCommand(article.Id.Value, "receive", 3, "Supplier delivery", null, null, "Refurbished"),
            CancellationToken.None);

        var persistedLots = await stockItems.ListByArticleIdAsync(article.Id, CancellationToken.None);
        Assert.Single(persistedLots);
        Assert.Equal(5, persistedLots[0].CurrentQuantity);
    }

    [Fact]
    public async Task CreateStockMovementRejectsUnknownArticle()
    {
        var useCase = new CreateStockMovementUseCase(
            new InMemoryArticleRepository(),
            new InMemoryStockItemRepository(),
            new CountingUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(
            new CreateStockMovementCommand(Guid.NewGuid(), "receive", 10, "Supplier delivery", null, null, "New"),
            CancellationToken.None));
    }

    [Fact]
    public async Task CreateStockMovementRejectsInvalidMovementType()
    {
        var article = CreateMerchandiseArticle();
        var useCase = new CreateStockMovementUseCase(
            new InMemoryArticleRepository(article),
            new InMemoryStockItemRepository(),
            new CountingUnitOfWork());

        await Assert.ThrowsAsync<ArgumentException>(() => useCase.ExecuteAsync(
            new CreateStockMovementCommand(article.Id.Value, "transfer", 10, "Move", null, null, "New"),
            CancellationToken.None));
    }

    [Fact]
    public async Task DeleteArticleRejectsArticleWithStock()
    {
        var article = CreateMerchandiseArticle();
        var stockItem = CreateMerchandiseStockItem(article, PackagingLevel.New);
        stockItem.Receive(1, "Supplier delivery");
        var useCase = new DeleteArticleUseCase(
            new InMemoryArticleRepository(article),
            new InMemoryStockItemRepository(stockItem),
            new CountingUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() => useCase.ExecuteAsync(article.Id.Value, CancellationToken.None));
    }

    private static StockItem CreateMerchandiseStockItem(Article article, PackagingLevel packagingLevel)
    {
        return StockItem.Create(article, null, null, packagingLevel);
    }

    private static StockItem CreateFoodStockItem(Article article, DateOnly expirationDate)
    {
        return StockItem.Create(article, expirationDate, TakeawayAvailability.TakeawayOnly, null);
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

    private sealed class InMemoryArticleRepository(params Article[] initialArticles) : IArticleRepository
    {
        private readonly List<Article> articles = [.. initialArticles];

        public Task<IReadOnlyList<Article>> ListAsync(string? nameSearch, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<Article>>(articles);
        }

        public Task<Article?> GetByIdAsync(ArticleId id, CancellationToken cancellationToken)
        {
            return Task.FromResult(articles.SingleOrDefault(article => article.Id == id));
        }

        public Task<bool> ExistsByReferenceAsync(Ean13Reference reference, ArticleId? excludedArticleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(articles.Any(article =>
                article.Reference == reference && (!excludedArticleId.HasValue || article.Id != excludedArticleId.Value)));
        }

        public void Add(Article article)
        {
            articles.Add(article);
        }

        public void Replace(Article existingArticle, Article replacementArticle)
        {
            Remove(existingArticle);
            Add(replacementArticle);
        }

        public void Remove(Article article)
        {
            articles.Remove(article);
        }
    }

    private sealed class InMemoryStockItemRepository(params StockItem[] initialStockItems) : IStockItemRepository
    {
        private readonly List<StockItem> stockItems = [.. initialStockItems];

        public Task<IReadOnlyList<StockItem>> ListByArticleIdAsync(ArticleId articleId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<StockItem>>(
                stockItems.Where(stockItem => stockItem.ArticleId == articleId).ToList());
        }

        public Task<bool> ExistsForArticleAsync(ArticleId articleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(stockItems.Any(stockItem => stockItem.ArticleId == articleId));
        }

        public void Add(StockItem stockItem)
        {
            stockItems.Add(stockItem);
        }
    }

    private sealed class CountingUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalls++;
            return Task.FromResult(0);
        }
    }
}
