using InventoryManagement.Application.Abstractions;

namespace InventoryManagement.Application.Articles.UseCases;

public sealed class ListArticlesUseCase(IArticleRepository articles)
{
    public async Task<IReadOnlyList<ArticleDto>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var result = await articles.ListAsync(cancellationToken);
        return result.Select(ArticleMapper.ToDto).ToList();
    }
}
