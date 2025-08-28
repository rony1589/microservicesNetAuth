namespace ProductsService.Tests;

using System.Collections.Generic;
using System.Linq;
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

public class ListProductsHandlerTests
{
    [Fact]
    public async Task List_Should_Return_Dtos()
    {
        var repo = new Mock<IProductRepository>();
        var mapper = new Mock<IMapper>();

        var entities = new List<Product>
        {
            new () { Name = "A", Price = 1m, Status = true, Category = "C", Description = "D" },
            new () { Name = "B", Price = 2m, Status = true, Category = "C", Description = "D" }
        };
        repo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(entities);

        mapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
              .Returns((Product p) => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Category, p.Status));

        var handler = new ListProductsHandler(repo.Object, mapper.Object);
        var dtos = (await handler.Handle(new ListProductsQuery(), CancellationToken.None)).ToList();

        dtos.Should().HaveCount(2);
        dtos[0].Name.Should().Be("A");
        dtos[1].Price.Should().Be(2m);
    }
}
