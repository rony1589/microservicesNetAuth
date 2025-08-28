namespace UsersService.Tests;

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

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Register_Should_Create_User_And_Return_Dto()
    {
        var repo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();
        var hasher = new Mock<IPasswordHasher>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        hasher.Setup(h => h.Hash(It.IsAny<string>(), out It.Ref<string>.IsAny))
              .Callback(new HashCallback((string _, out string salt) => salt = "SALT"))
              .Returns("HASH");

        mapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
              .Returns((User u) => new UserDto(u.Id, u.Email, u.Name, u.Role.ToString(), u.IsActive));

        var handler = new RegisterUserCommandHandler(repo.Object, uow.Object, hasher.Object, mapper.Object);

        var cmd = new RegisterUserCommand("new@demo.com", "New", "P@ssw0rd", "Admin");

        var dto = await handler.Handle(cmd, CancellationToken.None);

        dto.Email.Should().Be("new@demo.com");
        dto.Role.Should().Be("Admin");
        repo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_Should_Fail_When_Email_Exists()
    {
        var repo = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();
        var hasher = new Mock<IPasswordHasher>();
        var mapper = new Mock<IMapper>();

        repo.Setup(r => r.FindByEmailAsync("exists@demo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = "exists@demo.com", Role = UserRole.Usuario });

        var handler = new RegisterUserCommandHandler(repo.Object, uow.Object, hasher.Object, mapper.Object);

        var cmd = new RegisterUserCommand("exists@demo.com", "Dup", "x", "Usuario");

        await FluentActions
            .Invoking(() => handler.Handle(cmd, CancellationToken.None))
            .Should().ThrowAsync<System.Exception>()
            .WithMessage("Email already exists");
    }

    private delegate void HashCallback(string password, out string salt);
}
