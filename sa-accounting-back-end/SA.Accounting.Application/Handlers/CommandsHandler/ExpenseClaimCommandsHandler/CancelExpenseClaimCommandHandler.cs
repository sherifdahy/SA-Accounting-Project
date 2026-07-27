using SA.Accounting.Application.Commands.ExpenseClaim;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;
using SA.Accounting.Infrastructure.Presistance.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimCommandsHandler;

public class CancelExpenseClaimHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelExpenseClaimCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(CancelExpenseClaimCommand command,CancellationToken cancellationToken)
    {
        var claim = await _unitOfWork.ExpenseClaims
            .FindAsync(x => x.Id == command.Id,[], cancellationToken);

        if (claim is null)
            return Result.Failure(ExpenseClaimErrors.NotFound);

        if (claim.CurrentState == ExpenseClaimState.Settled || claim.CurrentState == ExpenseClaimState.Cancelled)
            return Result.Failure(ExpenseClaimErrors.CannotCancel);

        var fromState = claim.CurrentState;
        claim.CurrentState = ExpenseClaimState.Cancelled;

        claim.Histories.Add(new ExpenseClaimHistory
        {
            ExpenseClaimId = claim.Id,
            FromState = fromState,
            ToState = ExpenseClaimState.Cancelled,
            Note = "Claim cancelled."
        });

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
