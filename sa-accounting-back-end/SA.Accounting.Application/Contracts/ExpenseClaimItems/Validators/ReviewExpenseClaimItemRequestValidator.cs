using SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;
using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaimItems.Validators;


public class ReviewExpenseClaimItemRequestValidator : AbstractValidator<ReviewExpenseClaimItemRequest>
{
    public ReviewExpenseClaimItemRequestValidator()
    {
        RuleFor(x => x.ExpenseClaimItemId)
            .GreaterThan(0);

        RuleFor(x => x.State)
            .IsInEnum()
            .Must(s => s == ExpenseClaimItemState.Approved
                    || s == ExpenseClaimItemState.Rejected)
            .WithMessage("Item state must be Approved or Rejected.");

        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .MaximumLength(1000)
            .When(x => x.State == ExpenseClaimItemState.Rejected)
            .WithMessage("Rejection reason is required when item is rejected.");

        RuleFor(x => x.RejectionReason)
            .MaximumLength(1000);
    }
}
