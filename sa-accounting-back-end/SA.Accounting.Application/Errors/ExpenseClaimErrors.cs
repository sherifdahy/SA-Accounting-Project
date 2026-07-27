using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Errors;

public static class ExpenseClaimErrors
{
    public static readonly Error NotFound = new(
        "ExpenseClaim.NotFound",
        "Expense claim was not found.",
        StatusCodes.Status404NotFound);

    public static readonly Error CannotUpdate = new(
        "ExpenseClaim.CannotUpdate",
        "Expense claim can not be Update dou to Expense Claim [Draft] or [ReturnedForEdit] state.",
        StatusCodes.Status400BadRequest);

    public static readonly Error CannotReview = new(
        "ExpenseClaim.CannotReview",
        "Expense claim can be reviewed only when it is in Submitted state.",
        StatusCodes.Status400BadRequest);

    public static readonly Error EmptyItems = new(
        "ExpenseClaim.EmptyItems",
        "Expense claim must have at least one item.",
        StatusCodes.Status400BadRequest);

    public static readonly Error ReviewMustCoverAllItems = new(
        "ExpenseClaim.ReviewMustCoverAllItems",
        "All claim items must be reviewed (Approved or Rejected). No item can remain Pending.",
        StatusCodes.Status400BadRequest);

    public static readonly Error CannotSubmit = new(
        "ExpenseClaim.CannotSubmit",
        "Expense claim can be submitted only from Draft or ReturnedForEdit state.",
        StatusCodes.Status400BadRequest);

    public static readonly Error InvalidItemState = new(
        "ExpenseClaim.InvalidItemState",
        "Each item must be reviewed as Approved or Rejected.",
        StatusCodes.Status400BadRequest);

    public static readonly Error CannotReturnForEdit = new(
        "ExpenseClaim.CannotReturnForEdit",
        "Expense claim can be returned for edit only from Submitted state.",
        StatusCodes.Status400BadRequest);

    public static readonly Error CannotSettle = new(
        "ExpenseClaim.CannotSettle",
        "Expense claim can be settled only when it is Approved or PartiallyApproved.",
        StatusCodes.Status400BadRequest);

    ///////////////



    public static readonly Error NotEditable = new(
        "ExpenseClaim.NotEditable",
        "Expense claim can be edited only when it is in Draft or ReturnedForEdit state.",
        StatusCodes.Status400BadRequest);

    public static readonly Error InvalidItemReference = new(
        "ExpenseClaim.InvalidItemReference",
        "One or more items do not belong to this expense claim.",
        StatusCodes.Status400BadRequest);


    public static readonly Error RejectionReasonRequired = new(
        "ExpenseClaim.RejectionReasonRequired",
        "Rejection reason is required for rejected items.",
        StatusCodes.Status400BadRequest);


    public static readonly Error CannotCancel = new(
        "ExpenseClaim.CannotCancel",
        "Expense claim cannot be cancelled in its current state.",
        StatusCodes.Status400BadRequest);

    public static readonly Error ReasonRequired = new(
        "ExpenseClaim.ReasonRequired",
        "Reason is required.",
        StatusCodes.Status400BadRequest);

    public static readonly Error AlreadySettled = new(
        "ExpenseClaim.AlreadySettled",
        "Expense claim has already been settled.",
        StatusCodes.Status409Conflict);

    public static readonly Error NoApprovedItems = new(
        "ExpenseClaim.NoApprovedItems",
        "Cannot settle a claim with no approved items.",
        StatusCodes.Status400BadRequest);

    public static readonly Error InsufficientCustodyBalance = new(
        "ExpenseClaim.InsufficientCustodyBalance",
        "Custody balance is insufficient to settle this claim.",
        StatusCodes.Status400BadRequest);
}
