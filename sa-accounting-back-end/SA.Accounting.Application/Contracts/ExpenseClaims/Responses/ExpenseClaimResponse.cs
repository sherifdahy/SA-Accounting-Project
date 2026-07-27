using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaims.Responses;

public record ExpenseClaimResponse(
    int Id,
    string Number,
    DateTime ClaimDate,
    string? Note,
    ExpenseClaimState CurrentState,
    int UserId,
    string UserName,
    decimal TotalAmount,
    decimal ApprovedAmount,
    decimal RejectedAmount,
    decimal PendingAmount,
    IReadOnlyList<ExpenseClaimItemResponse> Items,
    //IReadOnlyList<ExpenseClaimHistoryResponse> Histories,
    DateTime CreatedAt
);
