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
    public async Task GetStockReturnsZeroWhenArticleHasNoStockItem()
    {
        var article = CreateMerchandiseArticle();
        var articles = new InMemoryArticleRepository(article);
        var stockItems = new InMemoryStockItemRepository();
        var useCase = new GetStockByArticleIdUseCase(articles, stockItems);

        var stock = await useCase.ExecuteAsync(article.Id.Value, CancellationToken.None);

        Assert.Equal(article.Id.Value, stock.ArticleId);
        Assert.Equal(0, stock.CurrentQuantity);
        Assert.Equal(0, stock.SellableQuantity);
        Assert.Empty(stock.Movements);
    }

    [Fact]
    public async Task GetStockReturnsMovementHistory()
    {
        var article = CreateMerchandiseArticle();
        var stockItem = StockItem.Create(article.Id);
        stockItem.Receive(10, "Supplier delivery");
        stockItem.Remove(3, "Damaged");
        var useCase = new GetStockByArticleIdUseCase(
            new InMemoryArticleRepository(article),
            new InMemoryStockItemRepository(stockItem));

        var stock = await useCase.ExecuteAsync(article.Id.Value, CancellationToken.None);

        Assert.Equal(7, stock.CurrentQuantity);
        Assert.Equal(7, stock.SellableQuantity);
        Assert.Collection(
            stock.Movements.OrderBy(movement => movement.QuantityBefore),
            movement =>
            {
                Assert.Equal("receive", movement.Type);
                Assert.Equal(0, movement.QuantityBefore);
                Assert.Equal(10, movement.QuantityAfter);
            },
            movement =>
            {
                Assert.Equal("remove", movement.Type);
                Assert.Equal(10, movement.QuantityBefore);
                Assert.Equal(7, movement.QuantityAfter);
            });
    }

    [Fact]
    public async Task GetStockReturnsNoSellableQuantityForUnsellableMerchandise()
    {
        var article = CreateMerchandiseArticle(PackagingLevel.Unsellable);
        var stockItem = StockItem.Create(article.Id);
        stockItem.Receive(10, "Supplier delivery");
        var useCase = new GetStockByArticleIdUseCase(
            new InMemoryArticleRepository(article),
            new InMemoryStockItemRepository(stockItem));

        var stock = await useCase.ExecuteAsync(article.Id.Value, CancellationToken.None);

        Assert.Equal(10, stock.CurrentQuantity);
        Assert.Equal(0, stock.SellableQuantity);
    }

    [Fact]
    public async Task CreateStockMovementCreatesStockItemAndPersistsMovement()
    {
        var article = CreateMerchandiseArticle();
        var articles = new InMemoryArticleRepository(article);
        var stockItems = new InMemoryStockItemRepository();
        var unitOfWork = new CountingUnitOfWork();
        var useCase = new CreateStockMovementUseCase(articles, stockItems, unitOfWork);

        var movement = await useCase.ExecuteAsync(
            new CreateStockMovementCommand(article.Id.Value, "receive", 10, "Supplier delivery"),
            CancellationToken.None);

        var persistedStockItem = await stockItems.GetByArticleIdAsync(article.Id, CancellationToken.None);
        Assert.NotNull(persistedStockItem);
        Assert.Equal(10, persistedStockItem.CurrentQuantity);
        Assert.Equal("receive", movement.Type);
        Assert.Equal(0, movement.QuantityBefore);
        Assert.Equal(10, movement.QuantityAfter);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateStockMovementRejectsUnknownArticle()
    {
        var useCase = new CreateStockMovementUseCase(
            new InMemoryArticleRepository(),
            new InMemoryStockItemRepository(),
            new CountingUnitOfWork());

        await Assert.ThrowsAsync<NotFoundException>(() => useCase.ExecuteAsync(
            new CreateStockMovementCommand(Guid.NewGuid(), "receive", 10, "Supplier delivery"),
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
            new CreateStockMovementCommand(article.Id.Value, "transfer", 10, "Move"),
            CancellationToken.None));
    }

    [Fact]
    public async Task DeleteArticleRejectsArticleWithStock()
    {
        var article = CreateMerchandiseArticle();
        var articles = new InMemoryArticleRepository(article);
        var stockItems = new InMemoryStockItemRepository();
        var stockItem = StockItem.Create(article.Id);
        stockItem.Receive(1, "Supplier delivery");
        stockItems.Add(stockItem);
        var useCase = new DeleteArticleUseCase(articles, stockItems, new CountingUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() => useCase.ExecuteAsync(article.Id.Value, CancellationToken.None));
    }

    private static Article CreateMerchandiseArticle(PackagingLevel packagingLevel = PackagingLevel.New)
    {
        return Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            ArticleCategory.Merchandise,
            new Money(100),
            expirationDate: null,
            takeawayAvailability: null,
            packagingLevel: packagingLevel);
    }

    private sealed class InMemoryArticleRepository(params Article[] initialArticles) : IArticleRepository
    {
        private readonly List<Article> articles = [.. initialArticles];

        public Task<IReadOnlyList<Article>> ListAsync(CancellationToken cancellationToken)
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

        public Task<StockItem?> GetByArticleIdAsync(ArticleId articleId, CancellationToken cancellationToken)
        {
            return Task.FromResult(stockItems.SingleOrDefault(stockItem => stockItem.ArticleId == articleId));
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
