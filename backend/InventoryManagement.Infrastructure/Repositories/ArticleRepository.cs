using InventoryManagement.Application.Abstractions;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

internal sealed class ArticleRepository(AppDbContext dbContext) : IArticleRepository
{
    public async Task<IReadOnlyList<Article>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Articles
            .AsNoTracking()
            .OrderBy(article => article.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Article?> GetByIdAsync(ArticleId id, CancellationToken cancellationToken)
    {
        return dbContext.Articles.FirstOrDefaultAsync(article => article.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByReferenceAsync(Ean13Reference reference, ArticleId? excludedArticleId, CancellationToken cancellationToken)
    {
        return dbContext.Articles.AnyAsync(
            article => article.Reference == reference && (!excludedArticleId.HasValue || article.Id != excludedArticleId.Value),
            cancellationToken);
    }

    public void Add(Article article)
    {
        dbContext.Articles.Add(article);
    }

    public void Replace(Article existingArticle, Article replacementArticle)
    {
        dbContext.Entry(existingArticle).State = EntityState.Detached;
        dbContext.Articles.Update(replacementArticle);
    }

    public void Remove(Article article)
    {
        dbContext.Articles.Remove(article);
    }
}
