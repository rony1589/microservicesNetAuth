namespace UsersService.Application.DTOs;
public class TokenResponseDto
{
    public string AccessToken { get; set; } = default!;
    public UserDto User { get; set; } = default!;
}
