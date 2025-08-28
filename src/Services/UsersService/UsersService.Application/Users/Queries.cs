using AutoMapper;
using MediatR;
using UsersService.Application.DTOs;
using UsersService.Application.Abstractions;

namespace UsersService.Application.Users.Queries;

// Query estandarizada: obtener usuario por Id
public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

// Listado de usuarios (solo Admin desde el controller)
public sealed record ListUsersQuery() : IRequest<IEnumerable<UserDto>>;

// Handler: GetUserById
public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(IUserRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        // Asumimos que IUserRepository expone un método GetByIdAsync(Guid, CancellationToken)
        var entity = await _repo.GetByIdAsync(request.UserId, ct);
        return entity is null ? null : _mapper.Map<UserDto>(entity);
    }
}

// Handler: ListUsers
public sealed class ListUsersHandler : IRequestHandler<ListUsersQuery, IEnumerable<UserDto>>
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;

    public ListUsersHandler(IUserRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> Handle(ListUsersQuery request, CancellationToken ct)
    {
        // Asumimos que IUserRepository expone un método ListAsync(CancellationToken)
        var items = await _repo.ListAsync(ct);
        return _mapper.Map<IEnumerable<UserDto>>(items);
    }
}
