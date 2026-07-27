using SA.Accounting.Application.Contracts.ExpenseClaimItems.Validators;
using SA.Accounting.Application.Contracts.ExpenseClaims.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaims.Validators;

public class ExpenseClaimRequestValidator : AbstractValidator<ExpenseClaimRequest>
{
    public ExpenseClaimRequestValidator()
    {

        RuleFor(x => x.ClaimDate)
            .NotEmpty()
            .Must(d => d <= DateTime.UtcNow.AddDays(1))
            .WithMessage("ClaimDate cannot be in the future.");

        RuleFor(x => x.Note)
            .MaximumLength(1000);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new ExpenseClaimItemRequestValidator());
    }
}
