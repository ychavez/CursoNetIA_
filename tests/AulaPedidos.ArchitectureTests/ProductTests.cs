using AulaPedidos.Domain;
using Xunit;

namespace AulaPedidos.ArchitectureTests;

public sealed class ProductTests
{
    [Theory]
    [InlineData("d15c06ec-31e7-40a1-ad63-483374459ead", "Notebook", "SKU-001")]
    [InlineData("00000000-0000-0000-0000-000000000000", "", "")]
    [InlineData("d15c06ec-31e7-40a1-ad63-483374459ead", "  Notebook  ", "  sKu-001  ")]
    public void ConstructorPreservesProvidedValues(string idValue, string name, string sku)
    {
        var id = Guid.Parse(idValue);

        var product = new Product(id, name, sku);

        Assert.Equal(id, product.Id);
        Assert.Equal(name, product.Name);
        Assert.Equal(sku, product.Sku);
    }

    [Fact]
    public void PublicContractContainsOnlyReadOnlyIdNameAndSku()
    {
        var productType = typeof(Product);

        Assert.True(productType.IsPublic);
        Assert.Equal(new[] { "Id", "Name", "Sku" },
            productType.GetProperties().Select(property => property.Name).Order());
        Assert.Equal(typeof(Guid), productType.GetProperty(nameof(Product.Id))!.PropertyType);
        var nullability = new System.Reflection.NullabilityInfoContext();
        foreach (var name in new[] { nameof(Product.Name), nameof(Product.Sku) })
        {
            var property = productType.GetProperty(name)!;
            Assert.Equal(typeof(string), property.PropertyType);
            Assert.Equal(System.Reflection.NullabilityState.NotNull, nullability.Create(property).ReadState);
        }
        Assert.All(productType.GetProperties(), property => Assert.Null(property.SetMethod));
    }
}
