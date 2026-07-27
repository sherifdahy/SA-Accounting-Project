using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Errors;

public static class ExpenseClaimItemErrors
{
    public static readonly Error NotFound = new(
        "ExpenseClaimItem.NotFound",
        "Expense Claim Item not found.",
        StatusCodes.Status404NotFound);

    public static readonly Error AttachmentRequired = new(
        "ExpenseClaimItem.AttachmentRequired",
        "item require an attachments based on the selected category.",
        StatusCodes.Status400BadRequest);

    public static readonly Error CannotUpdate = new(
        "ExpenseClaimItem.CannotUpdate",
        "Can not update Expense Claim Item with Approved Status.",
        StatusCodes.Status400BadRequest);
}
