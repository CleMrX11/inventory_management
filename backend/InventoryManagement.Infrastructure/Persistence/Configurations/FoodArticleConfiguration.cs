using InventoryManagement.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

internal sealed class FoodArticleConfiguration : IEntityTypeConfiguration<FoodArticle>
{
    public void Configure(EntityTypeBuilder<FoodArticle> builder)
    {
    }
}
