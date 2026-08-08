using SA.Accounting.Application.Abstractions.Consts.RegExp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Validators.Shared;

public static class SSNValidationExtension
{
    public static IRuleBuilderOptions<T, string?> SSN<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .Matches(RegexPatterns.SSN);
    }
}
