using AulaPedidos.Application.Abstractions.Persistence;
using AulaPedidos.Domain;
using Mediator;

namespace AulaPedidos.Application.Products;

public sealed class CreateProductHandler(IRepository<Product> repository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateProductCommand, ProductDto>
{
    public async ValueTask<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var product = new Product(Guid.NewGuid(), command.Price, command.Description, command.AlternateDescription);
        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ProductDto.FromProduct(product);
    }
}

public sealed class UpdateProductHandler(IRepository<Product> repository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateProductCommand, ProductDto?>
{
    public async ValueTask<ProductDto?> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await repository.FirstOrDefaultAsync(product => product.Id == command.Id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        product.Update(command.Price, command.Description, command.AlternateDescription);
        repository.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ProductDto.FromProduct(product);
    }
}

public sealed class DeleteProductHandler(IRepository<Product> repository, IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteProductCommand, bool>
{
    public async ValueTask<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await repository.FirstOrDefaultAsync(product => product.Id == command.Id, cancellationToken);
        if (product is null)
        {
            return false;
        }

        repository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public sealed class GetProductByIdHandler(IRepository<Product> repository)
    : IQueryHandler<GetProductByIdQuery, ProductDto?>
{
    public async ValueTask<ProductDto?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await repository.FirstOrDefaultAsync(product => product.Id == query.Id, cancellationToken);
        return product is null ? null : ProductDto.FromProduct(product);
    }
}

public sealed class GetProductsHandler(IRepository<Product> repository)
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async ValueTask<IReadOnlyList<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await repository.GetAsync(cancellationToken: cancellationToken);
        return products.OrderBy(product => product.Id).Select(ProductDto.FromProduct).ToArray();
    }
}
