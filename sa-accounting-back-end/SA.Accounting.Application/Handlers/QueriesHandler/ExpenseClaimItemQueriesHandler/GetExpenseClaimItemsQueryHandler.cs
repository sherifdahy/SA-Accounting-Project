using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.ExpenseClaimItem;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.QueriesHandler.ExpenseClaimItemQueriesHandler;

public class GetExpenseClaimItemsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetExpenseClaimItemsQuery, Result<IReadOnlyList<ExpenseClaimItemResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<ExpenseClaimItemResponse>>> Handle(GetExpenseClaimItemsQuery query, CancellationToken cancellationToken)
    {
        var claim = await _unitOfWork.ExpenseClaims.FindAsync(x => x.Id == query.ClaimId);

        if (claim is null)
            return Result.Failure<IReadOnlyList<ExpenseClaimItemResponse>>(ExpenseClaimItemErrors.NotFound);
        
        var claimItems = await _unitOfWork.ExpenseClaimItems.FindAllAsync(x => x.ExpenseClaimId == query.ClaimId, [
                x=>x.Include(x=>x.Company),
                x=>x.Include(x=>x.ExpenseCategory),
                x=>x.Include(x=>x.Attachments)
            ], cancellationToken);

        var response = claimItems.Adapt<IReadOnlyList<ExpenseClaimItemResponse>>();

        return Result.Success(response);
    }
}
