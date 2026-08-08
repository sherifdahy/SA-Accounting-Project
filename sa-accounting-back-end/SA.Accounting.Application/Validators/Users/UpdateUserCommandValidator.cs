using SA.Accounting.Application.Commands.User;
using SA.Accounting.Application.Validators.Shared;

namespace SA.Accounting.Application.Validators.Users;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Name)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MinimumLength(3)
           .MaximumLength(50);

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .Password()
            .When(X=> string.IsNullOrEmpty(X.Password));

        RuleFor(x => x.PhoneNumber!)
            .PhoneNumber()
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.SSN)
            .SSN()
            .When(x => !string.IsNullOrWhiteSpace(x.SSN));

        RuleFor(x => x.CompanyIds)
            .UniqueValues()
            .When(x => x != null);

        RuleFor(x => x.Roles)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .UniqueValues();
    }
}
