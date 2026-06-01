using InventoryManagement.Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

internal sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(movement => movement.Id);

        builder.Property(movement => movement.Id)
            .HasConversion(id => id.Value, value => new StockMovementId(value))
            .ValueGeneratedNever();

        builder.Property(movement => movement.StockItemId)
            .HasConversion(id => id.Value, value => new StockItemId(value))
            .IsRequired();

        builder.Property(movement => movement.Type)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(movement => movement.Quantity)
            .IsRequired();

        builder.Property(movement => movement.QuantityBefore)
            .IsRequired();

        builder.Property(movement => movement.QuantityAfter)
            .IsRequired();

        builder.Property(movement => movement.Reason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(movement => movement.OccurredAt)
            .IsRequired();
    }
}
