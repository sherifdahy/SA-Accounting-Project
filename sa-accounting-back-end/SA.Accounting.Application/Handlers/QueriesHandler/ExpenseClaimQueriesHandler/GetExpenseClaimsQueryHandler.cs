using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Queries.ExpenseClaim;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Infrastructure.Presistance.Data;

namespace SA.Accounting.Application.Handlers.QueriesHandler.ExpenseClaimQueriesHandler;

public class GetExpenseClaimsHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetExpenseClaimsQuery, Result<IReadOnlyList<ExpenseClaimSummaryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<ExpenseClaimSummaryResponse>>> Handle(GetExpenseClaimsQuery query,CancellationToken cancellationToken)
    {
        var list = (await _unitOfWork.ExpenseClaims
            .FindAllAsync(x => true, [x=>x.Include(x=>x.User)],cancellationToken))
            .Select(x => new ExpenseClaimSummaryResponse(
                x.Id,
                x.Number,
                DateTime.Now.Date,
                x.CurrentState,
                x.UserId,
                x.User.Name,
                x.Items.Sum(i => (decimal?)i.Amount) ?? 0m,
                x.Items.Count,
                x.CreatedAt));

        return Result.Success<IReadOnlyList<ExpenseClaimSummaryResponse>>(list.Adapt<IReadOnlyList<ExpenseClaimSummaryResponse>>());
    }
}
