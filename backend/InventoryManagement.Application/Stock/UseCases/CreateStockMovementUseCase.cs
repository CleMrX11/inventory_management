using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.Stock.Commands;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;

namespace InventoryManagement.Application.Stock.UseCases;

public sealed class CreateStockMovementUseCase(
    IArticleRepository articles,
    IStockItemRepository stockItems,
    IUnitOfWork unitOfWork)
{
    public async Task<StockMovementDto> ExecuteAsync(CreateStockMovementCommand command, CancellationToken cancellationToken)
    {
        var articleId = new ArticleId(command.ArticleId);
        if (await articles.GetByIdAsync(articleId, cancellationToken) is null)
        {
            throw new NotFoundException("Article not found.");
        }

        var stockItem = await stockItems.GetByArticleIdAsync(articleId, cancellationToken);
        if (stockItem is null)
        {
            stockItem = StockItem.Create(articleId);
            stockItems.Add(stockItem);
        }

        var movementType = StockMovementTypeParser.Parse(command.Type);
        var movement = movementType switch
        {
            StockMovementType.Receive => stockItem.Receive(command.Quantity, command.Reason),
            StockMovementType.Remove => stockItem.Remove(command.Quantity, command.Reason),
            StockMovementType.Adjust => stockItem.AdjustTo(command.Quantity, command.Reason),
            _ => throw new ArgumentException("Stock movement type is invalid.", nameof(command)),
        };

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return StockMapper.ToDto(stockItem, movement);
    }
}
