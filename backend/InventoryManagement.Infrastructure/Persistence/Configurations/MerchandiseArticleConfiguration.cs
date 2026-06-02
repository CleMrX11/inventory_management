using InventoryManagement.Domain.Articles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagement.Infrastructure.Persistence.Configurations;

internal sealed class MerchandiseArticleConfiguration : IEntityTypeConfiguration<MerchandiseArticle>
{
    public void Configure(EntityTypeBuilder<MerchandiseArticle> builder)
    {
    }
}
