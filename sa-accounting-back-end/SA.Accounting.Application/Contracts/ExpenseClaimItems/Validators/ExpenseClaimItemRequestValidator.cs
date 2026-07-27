using SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;
using SA.Accounting.Application.Contracts.Files.Validators;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaimItems.Validators;

public class ExpenseClaimItemRequestValidator : AbstractValidator<ExpenseClaimItemRequest>
{
    public ExpenseClaimItemRequestValidator()
    {
        RuleFor(x => x.CompanyId)
            .GreaterThan(0);

        RuleFor(x => x.ExpenseCategoryId)
            .GreaterThan(0);

        RuleFor(x => x.Note)
            .MaximumLength(1000);

        RuleForEach(x => x.Files)
            .SetValidator(new FileSizeValidator())
            .SetValidator(new AllowedSignatureValidator());

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .PrecisionScale(18, 2, true);
    }
}
