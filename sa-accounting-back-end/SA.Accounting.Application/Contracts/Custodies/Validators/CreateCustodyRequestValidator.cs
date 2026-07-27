using SA.Accounting.Application.Contracts.Custodies.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.Custodies.Validators;

public class CreateCustodyRequestValidator : AbstractValidator<CreateCustodyRequest>
{
    public CreateCustodyRequestValidator()
    {
        RuleFor(x => x.Note)
            .MaximumLength(500);
    }
}
