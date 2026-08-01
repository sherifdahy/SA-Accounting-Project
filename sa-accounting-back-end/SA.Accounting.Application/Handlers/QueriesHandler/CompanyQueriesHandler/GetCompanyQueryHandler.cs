using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.Account.Responses;
using SA.Accounting.Application.Contracts.Company.Responses;
using SA.Accounting.Application.Contracts.Owner.Responses;
using SA.Accounting.Application.Contracts.Platform.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.Company;
using SA.Accounting.Core.Entities.Interfaces;
using System.Linq.Dynamic.Core;

namespace SA.Accounting.Application.Handlers.QueriesHandler.CompanyQueriesHandler;

public class GetCompanyQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCompanyQuary, Result<CompanyDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<CompanyDetailResponse>> Handle(GetCompanyQuary request, CancellationToken cancellationToken)
    {
        var company = await _unitOfWork.Companies.FindAsync(
            x => x.Id == request.Id,
            ["Owners", "Accounts.Platform.Selectors"],
            cancellationToken);

        if (company == null)
            return Result.Failure<CompanyDetailResponse>(CompanyErrors.NotFound);

        var response = company.Adapt<CompanyDetailResponse>();

        return Result.Success(response);
    }
}
