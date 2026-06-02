using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Globalization;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

internal sealed class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.ToTable("StockItems");

        builder.HasKey(stockItem => stockItem.Id);

        builder.Property(stockItem => stockItem.Id)
            .HasConversion(id => id.Value, value => new StockItemId(value))
            .ValueGeneratedNever();

        builder.Property(stockItem => stockItem.ArticleId)
            .HasConversion(id => id.Value, value => new ArticleId(value))
            .IsRequired();

        builder.HasIndex(stockItem => stockItem.ArticleId);

        builder.Property(stockItem => stockItem.ExpirationDate)
            .HasConversion(
                value => value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                value => value == null ? null : DateOnly.Parse(value, CultureInfo.InvariantCulture))
            .HasMaxLength(10);

        builder.Property(stockItem => stockItem.TakeawayAvailability)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(stockItem => stockItem.PackagingLevel)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(stockItem => stockItem.CurrentQuantity)
            .IsRequired();

        builder.HasOne<Article>()
            .WithMany()
            .HasForeignKey(stockItem => stockItem.ArticleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(stockItem => stockItem.Movements)
            .WithOne()
            .HasForeignKey(movement => movement.StockItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(stockItem => stockItem.Movements)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
