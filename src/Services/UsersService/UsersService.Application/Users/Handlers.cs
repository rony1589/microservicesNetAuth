using AutoMapper;
using MediatR;
using UsersService.Application.Abstractions;
using UsersService.Application.DTOs;
using UsersService.Application.Users.Commands;
using UsersService.Application.Users.Queries;
using UsersService.Domain.Entities;
using UsersService.Domain.Enums; 
using BuildingBlocks.Errors; 

namespace UsersService.Application.Users
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
    {
        private readonly IUserRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _hasher;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IUserRepository repo, IUnitOfWork uow, IPasswordHasher hasher, IMapper mapper)
        { _repo = repo; _uow = uow; _hasher = hasher; _mapper = mapper; }

        public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            if (await _repo.FindByEmailAsync(request.Email, ct) is not null)
                throw new ConflictAppException(ErrorCode.Conflict, "Email already exists");

            UserRole role = UserRole.Usuario; // Valor por defecto

            if (!string.IsNullOrEmpty(request.Role))
            {
                // Intentar parsear como string primero
                if (Enum.TryParse<UserRole>(request.Role, true, out var parsedRole))
                {
                    role = parsedRole;
                }
                // Si no funciona, intentar como número
                else if (int.TryParse(request.Role, out var roleNumber))
                {
                    if (Enum.IsDefined(typeof(UserRole), roleNumber))
                    {
                        role = (UserRole)roleNumber;
                    }
                }
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Name = request.Name,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _hasher.Hash(request.Password, out var salt);
            user.PasswordSalt = salt;

            await _repo.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<UserDto>(user);
        }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, TokenResponseDto>
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenFactory _jwt;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(IUserRepository repo, IPasswordHasher hasher, IJwtTokenFactory jwt, IMapper mapper)
        { _repo = repo; _hasher = hasher; _jwt = jwt; _mapper = mapper; }

        public async Task<TokenResponseDto> Handle(LoginUserCommand request, CancellationToken ct)
        {
            var user = await _repo.FindByEmailAsync(request.Email, ct)
                       ?? throw new UnauthorizedAppException(ErrorCode.AuthInvalidCredentials, "Invalid credentials");

            if (!_hasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAppException(ErrorCode.AuthInvalidCredentials, "Invalid credentials");

            var token = _jwt.Create(user, TimeSpan.FromHours(2));
            return new TokenResponseDto { AccessToken = token, User = _mapper.Map<UserDto>(user) };
        }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository repo, IMapper mapper)
        { _repo = repo; _mapper = mapper; }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            var user = await _repo.GetByIdAsync(request.UserId, ct);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }
    }

    public class ListUsersHandler : IRequestHandler<ListUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public ListUsersHandler(IUserRepository repo, IMapper mapper)
        { _repo = repo; _mapper = mapper; }

        public async Task<IEnumerable<UserDto>> Handle(ListUsersQuery request, CancellationToken ct)
        {
            var users = await _repo.ListAsync(ct);
            return users.Select(_mapper.Map<UserDto>);
        }
    }
}
