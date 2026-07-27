using SA.Accounting.Application.Contracts.Custodies.Requests;
using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.Custodies.Validators;

public class CreateCustodyMovementRequestValidator : AbstractValidator<CreateMovementRequest>
{
    public CreateCustodyMovementRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .Must(x => x != MovementType.ApprovedExpense)
            .WithMessage("ApprovedExpense movement is created only by settlement.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .PrecisionScale(18, 2, true);

        RuleFor(x => x.Note)
            .MaximumLength(500);

        RuleFor(x => x.Note)
            .NotEmpty()
            .When(x => x.Type == MovementType.AdjustmentIn ||
                       x.Type == MovementType.AdjustmentOut)
            .WithMessage("Adjustment movements require a note.");
    }
}
