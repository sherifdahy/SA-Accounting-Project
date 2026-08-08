using SA.Accounting.Application.Abstractions;
using SA.Accounting.Application.Abstractions.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Infrastructure.Services;

public class CompanyService(IUnitOfWork unitOfWork) : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> ValidateCompaniesAsync(IReadOnlyCollection<int> companyIds, CancellationToken cancellationToken = default)
    {
        var existingCompanies = await _unitOfWork.Companies
            .FindAllAsync(x => companyIds.Contains(x.Id), cancellationToken);

        var missingIds = companyIds.Except(existingCompanies.Select(x=>x.Id)).ToList();

        if (missingIds.Any())
        {
            return Result.Failure(
                new Error(
                    "Companies.NotFound",
                    $"The following company IDs do not exist: {string.Join(", ", missingIds)}",
                    404));
        }

        return Result.Success();
    }
}
