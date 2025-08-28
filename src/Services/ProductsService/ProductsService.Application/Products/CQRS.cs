using MediatR;
using ProductsService.Application.DTOs;

namespace ProductsService.Application.Products.CQRS
{
    // Interfaz común para reutilizar validaciones
    public interface IProductWrite
    {
        string Name { get; }
        string Description { get; }
        decimal Price { get; }
        string Category { get; }
    }

    // Create no exige Status (queda por defecto como Activo en la entidad)
    public record CreateProductCommand(
        string Name,
        string Description,
        decimal Price,
        string Category
    ) : IRequest<ProductDto>, IProductWrite;

    // Update ahora usa Status en lugar de IsActive
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string Description,
        decimal Price,
        string Category,
        Boolean Status
    ) : IRequest<ProductDto>, IProductWrite;

    public record DeleteProductCommand(Guid Id) : IRequest<Unit>;
    public record GetProductQuery(Guid Id) : IRequest<ProductDto?>;
    public record ListProductsQuery() : IRequest<IEnumerable<ProductDto>>;
}
