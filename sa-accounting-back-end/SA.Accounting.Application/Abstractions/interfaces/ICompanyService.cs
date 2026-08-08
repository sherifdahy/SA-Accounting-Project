using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Abstractions.interfaces;

public interface ICompanyService
{
    Task<Result> ValidateCompaniesAsync(IReadOnlyCollection<int> companyIds, CancellationToken cancellationToken = default);
}
