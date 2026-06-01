using InventoryManagement.Application.Abstractions;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Repositories;

internal sealed class StockItemRepository(AppDbContext dbContext) : IStockItemRepository
{
    public Task<StockItem?> GetByArticleIdAsync(ArticleId articleId, CancellationToken cancellationToken)
    {
        return dbContext.StockItems
            .Include(stockItem => stockItem.Movements)
            .FirstOrDefaultAsync(stockItem => stockItem.ArticleId == articleId, cancellationToken);
    }

    public Task<bool> ExistsForArticleAsync(ArticleId articleId, CancellationToken cancellationToken)
    {
        return dbContext.StockItems.AnyAsync(stockItem => stockItem.ArticleId == articleId, cancellationToken);
    }

    public void Add(StockItem stockItem)
    {
        dbContext.StockItems.Add(stockItem);
    }
}
