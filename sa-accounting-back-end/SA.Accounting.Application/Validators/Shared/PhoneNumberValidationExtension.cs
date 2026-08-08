using SA.Accounting.Application.Abstractions.Consts.RegExp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Validators.Shared;

static class PhoneNumberValidationExtension
{
    public static IRuleBuilderOptions<T, string?> PhoneNumber<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Matches(RegexPatterns.PhoneNumber)
            .WithMessage("'{PropertyName}' is not in a valid format.");
    }
}
