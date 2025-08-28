using AutoMapper;
using MediatR;
using ProductsService.Application.Abstractions;
using ProductsService.Application.DTOs;
using ProductsService.Application.Products.CQRS;
using ProductsService.Domain.Entities;
using System;
using BuildingBlocks.Errors;

namespace ProductsService.Application.Products
{
    public class CreateProductHandler(IProductRepository repo, IUnitOfWork uow, IMapper mapper)
        : IRequestHandler<CreateProductCommand, ProductDto>
    {
        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
        {
            // Si tienes mapeo configurado puedes usar mapper.Map<Product>(request).
            // De lo contrario, construir explícitamente evita depender del perfil.
            var entity = mapper.Map<Product>(request);
            entity.Id = Guid.NewGuid();
            // entity.Status queda con el default de la entidad: ProductStatus.Activo

            await repo.AddAsync(entity, ct);
            await uow.SaveChangesAsync(ct);
            return mapper.Map<ProductDto>(entity);
        }
    }

    public class UpdateProductHandler(IProductRepository repo, IUnitOfWork uow, IMapper mapper)
        : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var entity = await repo.GetAsync(request.Id, ct);
            if (entity is null)
                throw new NotFoundAppException(ErrorCode.NotFound, "Product not found");

            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Price = request.Price;
            entity.Category = request.Category;
            entity.Status = request.Status;
            entity.UpdatedAt = DateTime.UtcNow;

            await uow.SaveChangesAsync(ct);
            return mapper.Map<ProductDto>(entity);
        }
    }

    public class DeleteProductHandler(IProductRepository repo, IUnitOfWork uow)
        : IRequestHandler<DeleteProductCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken ct)
        {
            var entity = await repo.GetAsync(request.Id, ct);
            if (entity != null)
            {
                repo.Remove(entity);
                await uow.SaveChangesAsync(ct);
            }
            return Unit.Value;
        }
    }

    public class GetProductHandler(IProductRepository repo, IMapper mapper)
        : IRequestHandler<GetProductQuery, ProductDto?>
    {
        public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken ct)
        {
            var entity = await repo.GetAsync(request.Id, ct);
            return entity is null ? null : mapper.Map<ProductDto>(entity);
        }
    }

    public class ListProductsHandler(IProductRepository repo, IMapper mapper)
        : IRequestHandler<ListProductsQuery, IEnumerable<ProductDto>>
    {
        public async Task<IEnumerable<ProductDto>> Handle(ListProductsQuery request, CancellationToken ct)
        {
            var list = await repo.ListAsync(ct);
            return list.Select(mapper.Map<ProductDto>);
        }
    }
}
