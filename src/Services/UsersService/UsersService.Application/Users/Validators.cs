using FluentValidation;
using UsersService.Application.Users.Commands;

namespace UsersService.Application.Users.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

        // Validación opcional para Role
        When(x => !string.IsNullOrEmpty(x.Role), () =>
        {
            RuleFor(x => x.Role)
                .Must(role => role!.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                             role!.Equals("Usuario", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Rol incorrecto");
        });
    }

}

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
