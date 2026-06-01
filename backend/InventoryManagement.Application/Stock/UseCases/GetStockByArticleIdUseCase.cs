using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Common;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Stock.UseCases;

public sealed class GetStockByArticleIdUseCase(IArticleRepository articles, IStockItemRepository stockItems)
{
    public async Task<StockDto> ExecuteAsync(Guid articleId, CancellationToken cancellationToken)
    {
        var id = new ArticleId(articleId);
        if (await articles.GetByIdAsync(id, cancellationToken) is null)
        {
            throw new NotFoundException("Article not found.");
        }

        var stockItem = await stockItems.GetByArticleIdAsync(id, cancellationToken);
        return stockItem is null
            ? new StockDto(articleId, CurrentQuantity: 0)
            : StockMapper.ToDto(stockItem);
    }
}
