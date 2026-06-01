using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Application.Abstractions;

public interface IStockItemRepository
{
    Task<StockItem?> GetByArticleIdAsync(ArticleId articleId, CancellationToken cancellationToken);
    Task<bool> ExistsForArticleAsync(ArticleId articleId, CancellationToken cancellationToken);
    void Add(StockItem stockItem);
}
