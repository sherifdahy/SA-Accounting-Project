using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Commands.ExpenseClaim;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimCommandsHandler;

public class SubmitExpenseClaimHandler(IUnitOfWork unitOfWork) : IRequestHandler<SubmitExpenseClaimCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(SubmitExpenseClaimCommand command,CancellationToken cancellationToken)
    {
        // check expense claim is exist
        var claim = await _unitOfWork.ExpenseClaims
            .FindAsync(x => x.Id == command.Id, [x=>x.Include(d=>d.Items)],cancellationToken);

        if (claim is null)
            return Result.Failure(ExpenseClaimErrors.NotFound);

        // check state of claim is ([draft] or [return for edit])
        if (claim.CurrentState != ExpenseClaimState.Draft && claim.CurrentState != ExpenseClaimState.ReturnedForEdit)
            return Result.Failure(ExpenseClaimErrors.CannotSubmit);

        // check expense claim must have items
        if (claim.Items.Count == 0)
            return Result.Failure(ExpenseClaimErrors.EmptyItems);

        var fromState = claim.CurrentState;

        // Reset items to Pending
        foreach (var item in claim.Items)
        {
            item.State = ExpenseClaimItemState.Pending;
            item.RejectionReason = null;
        }

        // Transition state
        claim.CurrentState = ExpenseClaimState.Submitted;

        // Add history
        claim.Histories.Add(new ExpenseClaimHistory
        {
            ExpenseClaimId = claim.Id,
            FromState = fromState,
            ToState = ExpenseClaimState.Submitted,
            Note = "Claim submitted for review."
        });

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
