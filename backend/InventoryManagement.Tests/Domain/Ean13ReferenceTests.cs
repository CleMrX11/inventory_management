using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Tests.Domain;

public sealed class Ean13ReferenceTests
{
    [Fact]
    public void ConstructorAcceptsValidEan13()
    {
        var reference = new Ean13Reference("3664824001524");

        Assert.Equal("3664824001524", reference.Value);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("ABCDEFGHIJKLM")]
    [InlineData("366482400152")]
    [InlineData("36648240015245")]
    public void ConstructorRejectsInvalidEan13(string value)
    {
        Assert.Throws<ArgumentException>(() => new Ean13Reference(value));
    }
}
