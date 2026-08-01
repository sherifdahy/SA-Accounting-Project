using FluentValidation;

namespace SA.Accounting.Application.Commands.Auth.Validators;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}
