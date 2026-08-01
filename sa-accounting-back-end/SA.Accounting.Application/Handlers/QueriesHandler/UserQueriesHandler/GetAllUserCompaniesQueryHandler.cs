using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.Company.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.User;
using SA.Accounting.Core.Abstractions;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Entities.Relations;
using System.Linq.Expressions;

namespace SA.Accounting.Application.Handlers.QueriesHandler.UserQueriesHandler;

public class GetAllUserCompaniesQueryHandler(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager): IRequestHandler<GetAllUserCompaniesQuery, Result<PaginatedList<CompanyResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<PaginatedList<CompanyResponse>>> Handle(GetAllUserCompaniesQuery request, CancellationToken cancellationToken)
    {
        if(!await _userManager.Users.AnyAsync(d=>d.Id == request.UserId))
            return Result.Failure<PaginatedList<CompanyResponse>>(UserErrors.NotFound);

        Expression<Func<UserCompany, bool>> expression = x => (string.IsNullOrEmpty(request.Filters.SearchValue) ||
             x.Company.Name!.Contains(request.Filters.SearchValue) ||
             x.Company.TaxFileNumber!.Contains(request.Filters.SearchValue) ||
             x.Company.TaxRegistrationNumber.Contains(request.Filters.SearchValue)) && 
             x.UserId == request.UserId && 
             x.Company.IsDeleted == false;

        var count = await _unitOfWork.UserCompanies.CountAsync(expression, cancellationToken);

        var userCompanies = await _unitOfWork.UserCompanies.FindAllAsync(
            expression,
            ["Company"],
            request.Filters.PageSize * (request.Filters.PageNumber - 1),
            request.Filters.PageSize,
            request.Filters.SortColumn,
            request.Filters.SortDirection,
            cancellationToken);
        
        return Result.Success(PaginatedList<CompanyResponse>.Create(userCompanies.Select(d=>d.Company).Adapt<List<CompanyResponse>>(),count,request.Filters.PageNumber,request.Filters.PageSize));
    }
}
