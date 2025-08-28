using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Application.DTOs;
using ProductsService.Application.Products.CQRS;

namespace ProductsService.Web.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductsController(IMediator mediator) { _mediator = mediator; }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<ProductDto>> List(CancellationToken ct)
        => await _mediator.Send(new ListProductsQuery(), ct);

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto?>> Get(Guid id, CancellationToken ct)
        => Ok(await _mediator.Send(new GetProductQuery(id), ct));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductCommand body, CancellationToken ct)
        => Ok(await _mediator.Send(body with { Id = id }, ct));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}
