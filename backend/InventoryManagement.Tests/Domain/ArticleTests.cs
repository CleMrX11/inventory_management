using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Tests.Domain;

public sealed class ArticleTests
{
    [Fact]
    public void CreateRejectsPriceIncludingTaxLowerThanPriceExcludingTax()
    {
        Assert.Throws<ArgumentException>(() => Article.Create(
            new Ean13Reference("4006381333931"),
            "Keyboard",
            new Money(120),
            new Money(100)));
    }
}
