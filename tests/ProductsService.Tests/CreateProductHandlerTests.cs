namespace ProductsService.Tests;

using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using ProductsService.Application.Abstractions;
using ProductsService.Application.DTOs;
using ProductsService.Application.Products;
using ProductsService.Application.Products.CQRS;
using ProductsService.Domain.Entities;
using Xunit;

public class CreateProductHandlerTests
{
    [Fact]
    public async Task Create_Should_Add_And_Return_Dto()
    {
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();

        mapper.Setup(m => m.Map<Product>(It.IsAny<CreateProductCommand>()))
              .Returns((CreateProductCommand c) => new Product
              {
                  Name = c.Name,
                  Description = c.Description,
                  Price = c.Price,
                  Category = c.Category,
                  Status = true
              });

        mapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
              .Returns((Product p) => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Category, p.Status));

        var handler = new CreateProductHandler(repo.Object, uow.Object, mapper.Object);

        var cmd = new CreateProductCommand(
             "Mouse",
             "Gamer",
             49.9m,
             "Peripherals"
         );

        var dto = await handler.Handle(cmd, CancellationToken.None);

        dto.Name.Should().Be("Mouse");
        repo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
