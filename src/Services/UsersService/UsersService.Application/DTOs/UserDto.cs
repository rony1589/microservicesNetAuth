namespace UsersService.Application.DTOs;
public record UserDto(Guid Id, string Email, string Name, string? Role, bool IsActive);
