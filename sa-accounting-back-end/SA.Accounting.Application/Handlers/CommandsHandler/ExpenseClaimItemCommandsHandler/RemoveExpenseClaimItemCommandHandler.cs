using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Commands.ExpenseClaimItem;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimItemCommandsHandler;

public class RemoveExpenseClaimItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RemoveExpenseClaimItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(RemoveExpenseClaimItemCommand command, CancellationToken cancellationToken)
    {
        // check claim item is exist
        if (await _unitOfWork.ExpenseClaimItems.FindAsync(x => x.Id == command.ClaimItemId, [x => x.Include(x => x.ExpenseClaim),x=>x.Include(x=>x.ExpenseCategory)], cancellationToken) is not ExpenseClaimItem claimItem)
            return Result.Failure(ExpenseClaimItemErrors.NotFound);

        // check claim item is ([pending] or [rejected])
        if (claimItem.State == ExpenseClaimItemState.Approved)
            return Result.Failure(ExpenseClaimItemErrors.CannotUpdate);

        // check claim is ([draft] or [returned for edit])
        if (claimItem.ExpenseClaim.CurrentState != ExpenseClaimState.Draft && claimItem.ExpenseClaim.CurrentState != ExpenseClaimState.ReturnedForEdit)
            return Result.Failure<ExpenseClaimItemResponse>(ExpenseClaimErrors.CannotUpdate);

        if (claimItem.ExpenseCategory.RequiresAttachment)
        {
            // remove files
        }

        _unitOfWork.ExpenseClaimItems.Delete(claimItem);
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
