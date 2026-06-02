using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Common;

namespace InventoryManagement.Domain.Stock;

public sealed class StockItem : AggregateRoot<StockItemId>
{
    private readonly List<StockMovement> movements = [];

    public ArticleId ArticleId { get; private set; }
    public DateOnly? ExpirationDate { get; private set; }
    public TakeawayAvailability? TakeawayAvailability { get; private set; }
    public PackagingLevel? PackagingLevel { get; private set; }
    public int CurrentQuantity { get; private set; }
    public IReadOnlyCollection<StockMovement> Movements => movements.AsReadOnly();

    private StockItem()
    {
    }

    private StockItem(
        StockItemId id,
        ArticleId articleId,
        DateOnly? expirationDate,
        TakeawayAvailability? takeawayAvailability,
        PackagingLevel? packagingLevel) : base(id)
    {
        ArticleId = articleId;
        ExpirationDate = expirationDate;
        TakeawayAvailability = takeawayAvailability;
        PackagingLevel = packagingLevel;
    }

    public static StockItem Create(
        Article article,
        DateOnly? expirationDate,
        TakeawayAvailability? takeawayAvailability,
        PackagingLevel? packagingLevel)
    {
        ValidateLotSpecificity(article.Category, expirationDate, takeawayAvailability, packagingLevel);
        return new StockItem(StockItemId.New(), article.Id, expirationDate, takeawayAvailability, packagingLevel);
    }

    public bool MatchesLot(DateOnly? expirationDate, TakeawayAvailability? takeawayAvailability, PackagingLevel? packagingLevel)
    {
        return ExpirationDate == expirationDate &&
            TakeawayAvailability == takeawayAvailability &&
            PackagingLevel == packagingLevel;
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

    private static void ValidateLotSpecificity(
        ArticleCategory category,
        DateOnly? expirationDate,
        TakeawayAvailability? takeawayAvailability,
        PackagingLevel? packagingLevel)
    {
        switch (category)
        {
            case ArticleCategory.FoodItem:
                if (!expirationDate.HasValue)
                {
                    throw new ArgumentException("Expiration date is required for food stock lots.", nameof(expirationDate));
                }

                if (!takeawayAvailability.HasValue)
                {
                    throw new ArgumentException("Takeaway availability is required for food stock lots.", nameof(takeawayAvailability));
                }

                if (packagingLevel.HasValue)
                {
                    throw new ArgumentException("Packaging level is only valid for merchandise stock lots.", nameof(packagingLevel));
                }

                if (!Enum.IsDefined(takeawayAvailability.Value))
                {
                    throw new ArgumentException("Takeaway availability is invalid.", nameof(takeawayAvailability));
                }

                break;
            case ArticleCategory.Merchandise:
                if (expirationDate.HasValue)
                {
                    throw new ArgumentException("Expiration date is only valid for food stock lots.", nameof(expirationDate));
                }

                if (takeawayAvailability.HasValue)
                {
                    throw new ArgumentException("Takeaway availability is only valid for food stock lots.", nameof(takeawayAvailability));
                }

                if (!packagingLevel.HasValue)
                {
                    throw new ArgumentException("Packaging level is required for merchandise stock lots.", nameof(packagingLevel));
                }

                if (!Enum.IsDefined(packagingLevel.Value))
                {
                    throw new ArgumentException("Packaging level is invalid.", nameof(packagingLevel));
                }

                break;
            default:
                throw new ArgumentException("Article category is invalid.", nameof(category));
        }
    }
}
