using InventoryManagement.Application.Abstractions;

namespace InventoryManagement.Application.Articles.UseCases;

public sealed class ListArticlesUseCase(IArticleRepository articles)
{
    public async Task<IReadOnlyList<ArticleDto>> ExecuteAsync(string? nameSearch, CancellationToken cancellationToken)
    {
        var result = await articles.ListAsync(nameSearch, cancellationToken);
        return result.Select(ArticleMapper.ToDto).ToList();
    }
}
