using SA.Accounting.Application.Contracts.ExpenseClaimItems.Validators;
using SA.Accounting.Application.Contracts.ExpenseClaims.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaims.Validators;

public class ReviewExpenseClaimRequestValidator : AbstractValidator<ReviewExpenseClaimRequest>
{
    public ReviewExpenseClaimRequestValidator()
    {
        RuleFor(x => x.Note)
            .MaximumLength(1000);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item must be reviewed.");

        RuleForEach(x => x.Items)
            .SetValidator(new ReviewExpenseClaimItemRequestValidator());

        RuleFor(x => x.Items)
            .Must(items =>
                items.Select(x => x.ExpenseClaimItemId).Distinct().Count() == items.Count)
            .WithMessage("Duplicate items are not allowed.");
    }
}
