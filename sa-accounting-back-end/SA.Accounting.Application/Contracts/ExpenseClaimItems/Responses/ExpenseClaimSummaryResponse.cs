using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;

public record ExpenseClaimSummaryResponse(
    int Id,
    string Number,
    DateTime ClaimDate,
    ExpenseClaimState CurrentState,
    int UserId,
    string UserName,
    decimal TotalAmount,
    int ItemsCount,
    DateTime CreatedAt
);
