using MediatR;
using UsersService.Application.DTOs;

namespace UsersService.Application.Users.Commands;

public record RegisterUserCommand(string Email, string Name, string Password, string Role) : IRequest<UserDto>;
public record LoginUserCommand(string Email, string Password) : IRequest<TokenResponseDto>;
