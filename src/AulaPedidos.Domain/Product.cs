namespace AulaPedidos.Domain;

public sealed class Product
{
    public Product(Guid id, decimal price, string? description, string? alternateDescription)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("El identificador no puede estar vacío.", nameof(id));
        }

        Id = id;
        Update(price, description, alternateDescription);
    }

    public Guid Id { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public string? AlternateDescription { get; private set; }

    public void Update(decimal price, string? description, string? alternateDescription)
    {
        Price = price;
        Description = description;
        AlternateDescription = alternateDescription;
    }
}
