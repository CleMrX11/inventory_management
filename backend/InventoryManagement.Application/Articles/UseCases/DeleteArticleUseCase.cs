using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.UseCases;

public sealed class DeleteArticleUseCase(IArticleRepository articles, IUnitOfWork unitOfWork)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var article = await articles.GetByIdAsync(new ArticleId(id), cancellationToken)
            ?? throw new NotFoundException("Article not found.");

        articles.Remove(article);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
