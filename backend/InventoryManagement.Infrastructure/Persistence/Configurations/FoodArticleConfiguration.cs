using System.Globalization;
using InventoryManagement.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

internal sealed class FoodArticleConfiguration : IEntityTypeConfiguration<FoodArticle>
{
    public void Configure(EntityTypeBuilder<FoodArticle> builder)
    {
        builder.Property(article => article.ExpirationDate)
            .HasConversion(
                value => value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                value => DateOnly.Parse(value, CultureInfo.InvariantCulture))
            .HasMaxLength(10);

        builder.Property(article => article.TakeawayAvailability)
            .HasConversion<string>()
            .HasMaxLength(30);
    }
}
