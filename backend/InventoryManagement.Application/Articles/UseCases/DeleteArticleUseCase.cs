using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.UseCases;

public sealed class DeleteArticleUseCase(
    IArticleRepository articles,
    IStockItemRepository stockItems,
    IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var articleId = new ArticleId(id);
        var article = await articles.GetByIdAsync(articleId, cancellationToken)
            ?? throw new NotFoundException("Article not found.");

        if (await stockItems.ExistsForArticleAsync(articleId, cancellationToken))
        {
            throw new ConflictException("Cannot delete an article with stock.");
        }

        articles.Remove(article);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
