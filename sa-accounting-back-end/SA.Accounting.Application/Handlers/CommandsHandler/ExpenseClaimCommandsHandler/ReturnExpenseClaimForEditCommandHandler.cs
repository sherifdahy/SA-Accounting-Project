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

public class ReturnExpenseClaimForEditHandler(IUnitOfWork unitOfWork) : IRequestHandler<ReturnExpenseClaimForEditCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(ReturnExpenseClaimForEditCommand command,CancellationToken cancellationToken)
    {
        var claim = await _unitOfWork.ExpenseClaims
            .FindAsync(x => x.Id == command.Id, cancellationToken);

        if (claim is null)
            return Result.Failure(ExpenseClaimErrors.NotFound);

        if (claim.CurrentState != ExpenseClaimState.Submitted)
            return Result.Failure(ExpenseClaimErrors.CannotReturnForEdit);

        var fromState = claim.CurrentState;
        claim.CurrentState = ExpenseClaimState.ReturnedForEdit;

        claim.Histories.Add(new ExpenseClaimHistory
        {
            ExpenseClaimId = claim.Id,
            FromState = fromState,
            ToState = ExpenseClaimState.ReturnedForEdit,
            Note = command.Request.Reason
        });

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
