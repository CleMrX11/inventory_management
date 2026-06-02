using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Stock.UseCases;

public sealed class GetStockByArticleIdUseCase(IArticleRepository articles, IStockItemRepository stockItems)
{
    public async Task<StockDto> ExecuteAsync(Guid articleId, CancellationToken cancellationToken)
    {
        var id = new ArticleId(articleId);
        var article = await articles.GetByIdAsync(id, cancellationToken);
        if (article is null)
        {
            throw new NotFoundException("Article not found.");
        }

        var articleStockItems = await stockItems.ListByArticleIdAsync(id, cancellationToken);
        return StockMapper.ToDto(article, articleStockItems);
    }
}
