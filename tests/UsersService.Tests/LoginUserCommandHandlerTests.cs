namespace UsersService.Tests;

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using UsersService.Application.Abstractions;
using UsersService.Application.DTOs;
using UsersService.Application.Users;
using UsersService.Application.Users.Commands;
using UsersService.Domain.Entities;
using UsersService.Domain.Enums;
using Xunit;

public class LoginUserCommandHandlerTests
{
    [Fact]
    public async Task Login_Should_Return_Token_And_UserDto()
    {
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtTokenFactory>();
        var mapper = new Mock<IMapper>();

        var user = new User { Id = Guid.NewGuid(), Email = "admin@demo.com", Name = "Admin", Role = UserRole.Admin, PasswordHash = "HASH", IsActive = true };

        repo.Setup(r => r.FindByEmailAsync("admin@demo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        hasher.Setup(h => h.Verify("P@ss", "HASH")).Returns(true);
        jwt.Setup(j => j.Create(user, It.IsAny<TimeSpan?>())).Returns("TOKEN123");
        mapper.Setup(m => m.Map<UserDto>(user))
              .Returns(new UserDto(user.Id, user.Email, user.Name, "Admin", true));

        var handler = new LoginUserCommandHandler(repo.Object, hasher.Object, jwt.Object, mapper.Object);

        var cmd = new LoginUserCommand("admin@demo.com", "P@ss");

        var result = await handler.Handle(cmd, CancellationToken.None);

        result.AccessToken.Should().Be("TOKEN123");
        result.User.Email.Should().Be("admin@demo.com");
    }

    [Fact]
    public async Task Login_Should_Fail_When_Invalid_Credentials()
    {
        var repo = new Mock<IUserRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtTokenFactory>();
        var mapper = new Mock<IMapper>();

        var user = new User { Id = Guid.NewGuid(), Email = "x@demo.com", PasswordHash = "HASH", Role = UserRole.Usuario, IsActive = true };
        repo.Setup(r => r.FindByEmailAsync("x@demo.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        hasher.Setup(h => h.Verify("bad", "HASH")).Returns(false);

        var handler = new LoginUserCommandHandler(repo.Object, hasher.Object, jwt.Object, mapper.Object);
        var cmd = new LoginUserCommand("x@demo.com", "bad");

        await FluentActions.Invoking(() => handler.Handle(cmd, CancellationToken.None))
            .Should().ThrowAsync<Exception>()
            .WithMessage("Invalid credentials");
    }
}
