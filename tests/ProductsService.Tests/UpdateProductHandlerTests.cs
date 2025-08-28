namespace ProductsService.Tests;

using System;
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

public class UpdateProductHandlerTests
{
    [Fact]
    public async Task Update_Should_Fail_When_Not_Found()
    {
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        var map = new Mock<IMapper>();

        repo.Setup(r => r.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateProductHandler(repo.Object, uow.Object, map.Object);

        var cmd = new UpdateProductCommand(Guid.NewGuid(), "X", "Y", 1m, "C", true);

        await FluentActions.Invoking(() => handler.Handle(cmd, CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage("Product not found");
    }

    [Fact]
    public async Task Update_Should_Save_Changes_And_Return_Dto()
    {
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        var map = new Mock<IMapper>();

        var entity = new Product
        {
            Id = Guid.NewGuid(),
            Name = "A",
            Description = "B",
            Price = 10m,
            Category = "Cat",
            Status = true
        };

        repo.Setup(r => r.GetAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        map.Setup(m => m.Map<ProductDto>(entity))
           .Returns(new ProductDto(entity.Id, "NEW", "NEW", 20m, "NEW", false));

        var handler = new UpdateProductHandler(repo.Object, uow.Object, map.Object);

        var cmd = new UpdateProductCommand(entity.Id, "NEW", "NEW", 20m, "NEW", false);

        var dto = await handler.Handle(cmd, CancellationToken.None);

        dto.Price.Should().Be(20m);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
