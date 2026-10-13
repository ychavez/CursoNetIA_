namespace AulaPedidos.Domain;

public sealed class Product
{
    public Product(Guid id, string name, string sku)
    {
        Id = id;
        Name = name;
        Sku = sku;
    }

    public Guid Id { get; }

    public string Name { get; }

    public string Sku { get; }
}
