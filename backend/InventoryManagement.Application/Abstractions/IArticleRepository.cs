using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Abstractions;

public interface IArticleRepository
{
    Task<IReadOnlyList<Article>> ListAsync(CancellationToken cancellationToken);
    Task<Article?> GetByIdAsync(ArticleId id, CancellationToken cancellationToken);
    Task<bool> ExistsByReferenceAsync(Ean13Reference reference, ArticleId? excludedArticleId, CancellationToken cancellationToken);
    void Add(Article article);
    void Replace(Article existingArticle, Article replacementArticle);
    void Remove(Article article);
}
