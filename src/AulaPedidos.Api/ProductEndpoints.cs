using System.Text.Json.Serialization;
using AulaPedidos.Application.Products;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AulaPedidos.Api;

public sealed record ProductRequest([property: JsonRequired] decimal Price, string? Description, string? AlternateDescription);

public static class ProductEndpoints
{
    public static void MapProducts(this IEndpointRouteBuilder endpoints)
    {
        var products = endpoints.MapGroup("/products").WithTags("Products");

        products.MapPost("/", async Task<Created<ProductDto>> (
            ProductRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var product = await sender.Send(
                new CreateProductCommand(request.Price, request.Description, request.AlternateDescription), cancellationToken);
            return TypedResults.Created($"/products/{product.Id}", product);
        });

        products.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
            TypedResults.Ok(await sender.Send(new GetProductsQuery(), cancellationToken)));

        products.MapGet("/{id:guid}", async Task<Results<Ok<ProductDto>, NotFound>> (
            Guid id, ISender sender, CancellationToken cancellationToken) =>
        {
            var product = await sender.Send(new GetProductByIdQuery(id), cancellationToken);
            return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
        });

        products.MapPut("/{id:guid}", async Task<Results<Ok<ProductDto>, NotFound>> (
            Guid id, ProductRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            var product = await sender.Send(
                new UpdateProductCommand(id, request.Price, request.Description, request.AlternateDescription), cancellationToken);
            return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
        });

        products.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (
            Guid id, ISender sender, CancellationToken cancellationToken) =>
            await sender.Send(new DeleteProductCommand(id), cancellationToken)
                ? TypedResults.NoContent() : TypedResults.NotFound());
    }
}
