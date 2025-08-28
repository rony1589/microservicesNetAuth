using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersService.Application.DTOs;
using UsersService.Application.Users.Commands;
using UsersService.Application.Users.Queries;

namespace UsersService.Web.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IMediator mediator, ILogger<UsersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // POST: api/users/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserCommand cmd, CancellationToken ct)
    {
        var created = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // POST: api/users/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginUserCommand cmd, CancellationToken ct)
        => Ok(await _mediator.Send(cmd, ct));

    // GET: api/users/{id}
    // Estandarizado: se pasa ID por ruta.
    // Regla:
    //  - Admin puede consultar cualquier usuario.
    //  - Usuario normal solo puede consultar su propio id (claim sub/NameIdentifier).
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto?>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        // Si NO es Admin, solo puede ver su propio perfil
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin)
        {
            var sid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(sid, out var currentId) || currentId != id)
                return Forbid(); // 403
        }

        var dto = await _mediator.Send(new GetUserByIdQuery(id), ct);
        if (dto is null) return NotFound();

        return Ok(dto);
    }

    // GET: api/users/all  (solo Admin)
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> All(CancellationToken ct)
        => Ok(await _mediator.Send(new ListUsersQuery(), ct));
}
