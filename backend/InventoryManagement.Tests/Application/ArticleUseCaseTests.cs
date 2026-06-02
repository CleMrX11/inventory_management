using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.Commands;
using InventoryManagement.Application.Articles.UseCases;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Tests.Application;

public sealed class ArticleUseCaseTests
{
    [Fact]
    public async Task CreateFoodArticleMapsFoodFields()
    {
        var useCase = new CreateArticleUseCase(new InMemoryArticleRepository(), new NoOpUnitOfWork());

        var article = await useCase.ExecuteAsync(
            new CreateArticleCommand(
                "4006381333931",
                "Sandwich",
                "FoodItem",
                4,
                "2026-06-30",
                "Both",
                PackagingLevel: null),
            CancellationToken.None);

        Assert.Equal("FoodItem", article.Category);
        Assert.Equal(4.2m, article.PriceIncludingTax);
        Assert.Equal("2026-06-30", article.ExpirationDate);
        Assert.Equal("Both", article.TakeawayAvailability);
        Assert.Null(article.PackagingLevel);
    }

    [Fact]
    public async Task CreateMerchandiseArticleMapsPackagingLevel()
    {
        var useCase = new CreateArticleUseCase(new InMemoryArticleRepository(), new NoOpUnitOfWork());

        var article = await useCase.ExecuteAsync(
            new CreateArticleCommand(
                "4006381333931",
                "Keyboard",
                "Merchandise",
                100,
                null,
                null,
                "Refurbished"),
            CancellationToken.None);

        Assert.Equal("Merchandise", article.Category);
        Assert.Equal(120, article.PriceIncludingTax);
        Assert.Equal("Refurbished", article.PackagingLevel);
        Assert.Null(article.ExpirationDate);
        Assert.Null(article.TakeawayAvailability);
    }

    private sealed class InMemoryArticleRepository : IArticleRepository
    {
        private readonly List<Article> articles = [];

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

    private sealed class NoOpUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
