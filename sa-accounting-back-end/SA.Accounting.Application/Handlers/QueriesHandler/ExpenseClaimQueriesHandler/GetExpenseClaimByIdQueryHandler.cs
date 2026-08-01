using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.ExpenseClaims.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.ExpenseClaim;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.QueriesHandler.ExpenseClaimQueriesHandler;

public class GetExpenseClaimByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetExpenseClaimByIdQuery, Result<ExpenseClaimResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<ExpenseClaimResponse>> Handle(GetExpenseClaimByIdQuery query,CancellationToken cancellationToken)
    {
        var claim = await _unitOfWork.ExpenseClaims
            .FindAsync(
                x => x.Id == query.Id,
                ["User", "Items.Company", "Items.ExpenseCategory", "Histories.CreatedBy"],
                cancellationToken);

        if (claim is null)
            return Result.Failure<ExpenseClaimResponse>(ExpenseClaimErrors.NotFound);

        return Result.Success(claim.Adapt<ExpenseClaimResponse>());
    }
}
