using AulaPedidos.Domain;
using Xunit;

namespace AulaPedidos.ArchitectureTests;

public sealed class ProductTests
{
    [Theory]
    [InlineData("0")]
    [InlineData("-1.25")]
    [InlineData("79228162514264337593543950335")]
    public void ProductPreservesValuesAndIdentityWhenUpdated(string priceText)
    {
        var id = Guid.NewGuid();
        var price = decimal.Parse(priceText, System.Globalization.CultureInfo.InvariantCulture);
        var product = new Product(id, price, "  descripción  ", null);
        Assert.Equal(id, product.Id);
        Assert.Equal(price, product.Price);
        Assert.Equal("  descripción  ", product.Description);
        Assert.Null(product.AlternateDescription);

        product.Update(price, null, "alternativa");
        Assert.Equal(id, product.Id);
        Assert.Equal(price, product.Price);
        Assert.Null(product.Description);
        Assert.Equal("alternativa", product.AlternateDescription);
    }

    [Fact]
    public void EmptyIdentityIsRejected() =>
        Assert.Throws<ArgumentException>(() => new Product(Guid.Empty, 0, null, null));
}
