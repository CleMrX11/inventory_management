using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.Commands;
using InventoryManagement.Application.Articles.UseCases;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Tests.Application;

public sealed class ArticleUseCaseTests
{
    [Fact]
    public async Task CreateFoodArticleMapsProductFields()
    {
        var useCase = new CreateArticleUseCase(new InMemoryArticleRepository(), new NoOpUnitOfWork());

        var article = await useCase.ExecuteAsync(
            new CreateArticleCommand(
                "4006381333931",
                "Sandwich",
                "FoodItem",
                4),
            CancellationToken.None);

        Assert.Equal("FoodItem", article.Category);
        Assert.Equal(4, article.PriceExcludingTax);
    }

    [Fact]
    public async Task CreateMerchandiseArticleMapsProductFields()
    {
        var useCase = new CreateArticleUseCase(new InMemoryArticleRepository(), new NoOpUnitOfWork());

        var article = await useCase.ExecuteAsync(
            new CreateArticleCommand(
                "4006381333931",
                "Keyboard",
                "Merchandise",
                100),
            CancellationToken.None);

        Assert.Equal("Merchandise", article.Category);
        Assert.Equal(100, article.PriceExcludingTax);
    }

    [Fact]
    public async Task ListArticlesSearchesByName()
    {
        var repository = new InMemoryArticleRepository();
        repository.Add(CreateArticle("4006381333931", "Keyboard"));
        repository.Add(CreateArticle("5901234123457", "Mouse"));
        var useCase = new ListArticlesUseCase(repository);

        var articles = await useCase.ExecuteAsync("key", CancellationToken.None);

        Assert.Single(articles);
        Assert.Equal("Keyboard", articles[0].Name);
    }

    private sealed class InMemoryArticleRepository : IArticleRepository
    {
        private readonly List<Article> articles = [];

        public Task<IReadOnlyList<Article>> ListAsync(string? nameSearch, CancellationToken cancellationToken)
        {
            var result = articles.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                result = result.Where(article => article.Name.Contains(nameSearch.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult<IReadOnlyList<Article>>(result.ToList());
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

    private static Article CreateArticle(string reference, string name)
    {
        return Article.Create(
            new Ean13Reference(reference),
            name,
            ArticleCategory.Merchandise,
            new Money(100));
    }

    private sealed class NoOpUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
