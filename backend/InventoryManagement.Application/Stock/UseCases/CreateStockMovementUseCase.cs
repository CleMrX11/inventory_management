using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles;
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
        var article = await articles.GetByIdAsync(articleId, cancellationToken);
        if (article is null)
        {
            throw new NotFoundException("Article not found.");
        }

        var expirationDate = ArticleSpecificityParser.ParseExpirationDate(command.ExpirationDate);
        var takeawayAvailability = ArticleSpecificityParser.ParseTakeawayAvailability(command.TakeawayAvailability);
        var packagingLevel = ArticleSpecificityParser.ParsePackagingLevel(command.PackagingLevel);
        var articleStockItems = await stockItems.ListByArticleIdAsync(articleId, cancellationToken);
        var stockItem = articleStockItems.SingleOrDefault(item =>
            item.MatchesLot(expirationDate, takeawayAvailability, packagingLevel));
        if (stockItem is null)
        {
            stockItem = StockItem.Create(article, expirationDate, takeawayAvailability, packagingLevel);
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
