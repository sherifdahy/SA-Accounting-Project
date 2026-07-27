using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.ExpenseClaimItem;
using SA.Accounting.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.QueriesHandler.ExpenseClaimItemQueriesHandler;

public class GetExpenseClaimItemQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetExpenseClaimItemQuery, Result<ExpenseClaimItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ExpenseClaimItemResponse>> Handle(GetExpenseClaimItemQuery query, CancellationToken cancellationToken)
    {
        var claimItem = await _unitOfWork.ExpenseClaimItems.FindAsync(x => x.Id == query.Id, [
                x=>x.Include(x=>x.Company),
                x=>x.Include(x=>x.ExpenseCategory),
                x=>x.Include(x=>x.Attachments)
            ], cancellationToken);

        if (claimItem is null)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseClaimItemErrors.NotFound);

        var response = claimItem.Adapt<ExpenseClaimItemResponse>();

        return Result.Success(response);
    }
}
