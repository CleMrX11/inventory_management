using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.UseCases;

public sealed class GetArticleByIdUseCase(IArticleRepository articles)
{
    public async Task<ArticleDto> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var article = await articles.GetByIdAsync(new ArticleId(id), cancellationToken);
        return article is null
            ? throw new NotFoundException("Article not found.")
            : ArticleMapper.ToDto(article);
    }
}
