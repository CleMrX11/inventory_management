using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Stock;

public sealed class StockItem : AggregateRoot<StockItemId>
{
    private readonly List<StockMovement> movements = [];

    public ArticleId ArticleId { get; private set; }
    public int CurrentQuantity { get; private set; }
    public IReadOnlyCollection<StockMovement> Movements => movements.AsReadOnly();

    private StockItem()
    {
    }

    private StockItem(StockItemId id, ArticleId articleId) : base(id)
    {
        ArticleId = articleId;
    }

    public static StockItem Create(ArticleId articleId)
    {
        return new StockItem(StockItemId.New(), articleId);
    }

    public StockMovement Receive(int quantity, string reason)
    {
        EnsurePositiveQuantity(quantity);
        return RecordMovement(StockMovementType.Receive, quantity, CurrentQuantity + quantity, reason);
    }

    public StockMovement Remove(int quantity, string reason)
    {
        EnsurePositiveQuantity(quantity);
        if (quantity > CurrentQuantity)
        {
            throw new InvalidOperationException("Cannot remove more stock than available.");
        }

        return RecordMovement(StockMovementType.Remove, quantity, CurrentQuantity - quantity, reason);
    }

    public StockMovement AdjustTo(int quantity, string reason)
    {
        if (quantity < 0)
        {
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(quantity));
        }

        return RecordMovement(StockMovementType.Adjust, quantity, quantity, reason);
    }

    private StockMovement RecordMovement(StockMovementType type, int quantity, int quantityAfter, string reason)
    {
        var trimmedReason = NormalizeReason(reason);
        var quantityBefore = CurrentQuantity;
        CurrentQuantity = quantityAfter;

        var movement = new StockMovement(
            StockMovementId.New(),
            Id,
            type,
            quantity,
            quantityBefore,
            quantityAfter,
            trimmedReason,
            DateTimeOffset.UtcNow);

        movements.Add(movement);
        return movement;
    }

    private static void EnsurePositiveQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Stock movement quantity must be greater than zero.", nameof(quantity));
        }
    }

    private static string NormalizeReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Stock movement reason is required.", nameof(reason));
        }

        return reason.Trim();
    }
}
