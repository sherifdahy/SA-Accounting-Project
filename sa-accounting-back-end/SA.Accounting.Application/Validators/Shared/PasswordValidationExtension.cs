using SA.Accounting.Application.Abstractions.Consts.RegExp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Validators.Shared;

public static class PasswordValidationExtension
{
    public static IRuleBuilderOptions<T, string?> Password<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Matches(RegexPatterns.Password)
            .WithMessage("'{PropertyName}' is not in a valid format.");
    }
}
