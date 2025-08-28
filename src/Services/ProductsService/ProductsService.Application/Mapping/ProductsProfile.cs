using AutoMapper;
using ProductsService.Application.DTOs;
using ProductsService.Application.Products.CQRS;
using ProductsService.Domain.Entities;

namespace ProductsService.Application.Mapping
{
    public class ProductsProfile : Profile
    {
        public ProductsProfile()
        {
            // Crear: toma Name/Description/Price/Category.
            // Status queda con el default de la entidad (ProductStatus.Activo).
            // Id/CreatedAt/UpdatedAt los maneja el handler y la BD.
            CreateMap<CreateProductCommand, Product>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());

            // Update: proyecta los campos editables. El handler controla UpdatedAt e Id existente.
            CreateMap<UpdateProductCommand, Product>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());

            CreateMap<Product, ProductDto>();
               
        }
    }
}
