using Mediator;

namespace AulaPedidos.Application.Products;

public sealed record CreateProductCommand(decimal Price, string? Description, string? AlternateDescription)
    : ICommand<ProductDto>;

public sealed record UpdateProductCommand(Guid Id, decimal Price, string? Description, string? AlternateDescription)
    : ICommand<ProductDto?>;

public sealed record DeleteProductCommand(Guid Id) : ICommand<bool>;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductDto?>;

public sealed record GetProductsQuery : IQuery<IReadOnlyList<ProductDto>>;
