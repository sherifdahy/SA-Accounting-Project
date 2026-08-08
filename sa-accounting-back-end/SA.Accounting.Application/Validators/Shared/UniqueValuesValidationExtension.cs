using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Validators.Shared;

public static class UniqueValuesValidationExtension
{
    public static IRuleBuilderOptions<T, IReadOnlyCollection<TItem>> UniqueValues<T, TItem>(this IRuleBuilder<T, IReadOnlyCollection<TItem>> ruleBuilder)
    {
        return ruleBuilder
            .Must(values => values.Distinct().Count() == values.Count)
            .WithMessage("'{PropertyName}' contains duplicate values.");
    }
}
