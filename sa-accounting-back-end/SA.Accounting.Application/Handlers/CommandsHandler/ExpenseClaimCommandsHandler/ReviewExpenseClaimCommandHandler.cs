using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Commands.ExpenseClaim;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseClaimCommandsHandler;
public class ReviewExpenseClaimHandler(IUnitOfWork unitOfWork) : IRequestHandler<ReviewExpenseClaimCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(ReviewExpenseClaimCommand command,CancellationToken cancellationToken)
    {
        // check expense claim is exist
        var claim = await _unitOfWork.ExpenseClaims
            .FindAsync(x => x.Id == command.Id, [d=>d.Include(i=>i.Items)],cancellationToken);

        if (claim is null)
            return Result.Failure(ExpenseClaimErrors.NotFound);

        // State check
        if (claim.CurrentState != ExpenseClaimState.Submitted)
            return Result.Failure(ExpenseClaimErrors.CannotReview);

        // All claim items must be reviewed (no missing)
        var claimItemIds = claim.Items.Select(i => i.Id).ToHashSet();
        var requestItemIdSet = command.Request.Items.Select(x=>x.ExpenseClaimItemId).ToHashSet();

        if (!claimItemIds.SetEquals(requestItemIdSet))
            return Result.Failure(ExpenseClaimErrors.ReviewMustCoverAllItems);

        // Apply review to each item
        var reviewDict = command.Request.Items.ToDictionary(i => i.ExpenseClaimItemId);
        foreach (var item in claim.Items)
        {
            var review = reviewDict[item.Id];
            item.State = review.State;
            item.RejectionReason = review.State == ExpenseClaimItemState.Rejected
                ? review.RejectionReason
                : null;
        }

        // 6. Calculate final claim state
        var approvedCount = claim.Items.Count(i => i.State == ExpenseClaimItemState.Approved);
        var rejectedCount = claim.Items.Count(i => i.State == ExpenseClaimItemState.Rejected);
        var totalCount = claim.Items.Count;

        var finalState = (approvedCount, rejectedCount) switch
        {
            var (a, _) when a == totalCount => ExpenseClaimState.Approved,
            _ => ExpenseClaimState.Rejected
        };

        var fromState = claim.CurrentState;
        claim.CurrentState = finalState;

        // Add history
        claim.Histories.Add(new ExpenseClaimHistory
        {
            ExpenseClaimId = claim.Id,
            FromState = fromState,
            ToState = finalState,
            Note = string.IsNullOrWhiteSpace(command.Request.Note)
                ? $"Reviewed: {approvedCount} approved, {rejectedCount} rejected."
                : command.Request.Note
        });

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
