using InventoryManagement.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

internal sealed class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(article => article.Id);

        builder.Property(article => article.Id)
            .HasConversion(id => id.Value, value => new ArticleId(value))
            .ValueGeneratedNever();

        builder.Property(article => article.Reference)
            .HasConversion(reference => reference.Value, value => new Ean13Reference(value))
            .HasMaxLength(13)
            .IsRequired();

        builder.HasIndex(article => article.Reference).IsUnique();

        builder.Property(article => article.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(article => article.Category)
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(ArticleCategory.Merchandise)
            .IsRequired();

        builder
            .HasDiscriminator(article => article.Category)
            .HasValue<FoodArticle>(ArticleCategory.FoodItem)
            .HasValue<MerchandiseArticle>(ArticleCategory.Merchandise);

        builder.Property(article => article.PriceExcludingTax)
            .HasConversion(price => price.Amount, amount => new Money(amount))
            .HasPrecision(12, 2)
            .IsRequired();

    }
}
