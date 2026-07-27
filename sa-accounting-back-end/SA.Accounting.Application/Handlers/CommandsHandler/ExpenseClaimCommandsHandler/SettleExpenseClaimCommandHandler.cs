using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Commands.ExpenseClaim;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;
using SA.Accounting.Services.Services;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimCommandsHandler;

public class SettleExpenseClaimHandler(IUnitOfWork unitOfWork,ICustodyBalanceCalculator custodyBalanceCalculator) : IRequestHandler<SettleExpenseClaimCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICustodyBalanceCalculator _custodyBalanceCalculator = custodyBalanceCalculator;

    public async Task<Result> Handle(SettleExpenseClaimCommand command,CancellationToken cancellationToken)
    {
        // Load claim with items
        var claim = await _unitOfWork.ExpenseClaims
            .FindAsync(x => x.Id == command.Id, [d=>d.Include(i=>i.Items)], cancellationToken);

        if (claim is null)
            return Result.Failure(ExpenseClaimErrors.NotFound);

        // State check
        if (claim.CurrentState != ExpenseClaimState.Approved)
            return Result.Failure(ExpenseClaimErrors.CannotSettle);

        // Idempotency check (defense in depth - DB has unique index too)
        var alreadySettled = _unitOfWork.CustodyMovements.IsExist(x => x.ExpenseClaimId == claim.Id && x.Type == MovementType.ApprovedExpense);

        if (alreadySettled)
            return Result.Failure(ExpenseClaimErrors.AlreadySettled);

        // Calculate approved amount
        var approvedItems = claim.Items
            .Where(i => i.State == ExpenseClaimItemState.Approved)
            .ToList();

        if (approvedItems.Count == 0)
            return Result.Failure(ExpenseClaimErrors.NoApprovedItems);

        var approvedAmount = approvedItems.Sum(i => i.Amount);

        // Get user's active custody
        var custody = await _unitOfWork.Custodies.FindAsync(x => x.UserId == claim.UserId,[],cancellationToken);

        if (custody is null)
            return Result.Failure(CustodyErrors.NotActive);

        // Check balance
        var currentBalance = await _custodyBalanceCalculator.GetBalanceAsync(custody.Id, cancellationToken);

        if (currentBalance < approvedAmount)
            return Result.Failure(ExpenseClaimErrors.InsufficientCustodyBalance);

        // Create the Movement (ApprovedExpense)
        var movement = new CustodyMovement
        {
            CustodyId = custody.Id,
            ExpenseClaimId = claim.Id,
            Type = MovementType.ApprovedExpense,
            Amount = approvedAmount,
            Note = $"Settlement for claim {claim.Number}"
        };

        await _unitOfWork.CustodyMovements.AddAsync(movement, cancellationToken);

        // Transition state
        var fromState = claim.CurrentState;
        claim.CurrentState = ExpenseClaimState.Settled;

        // Add history
        claim.Histories.Add(new ExpenseClaimHistory
        {
            ExpenseClaimId = claim.Id,
            FromState = fromState,
            ToState = ExpenseClaimState.Settled,
            Note = $"Settled {approvedAmount:F2} from custody {custody.Number}."
        });

        // Save (single transaction - all or nothing)
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
