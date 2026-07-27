using SA.Accounting.Application.Contracts.Files.Responses;
using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;

public record ExpenseClaimItemResponse(
    int Id,
    int CompanyId,
    string CompanyName,
    int ExpenseCategoryId,
    string ExpenseCategoryName,
    string? Note,
    decimal Amount,
    ExpenseClaimItemState State,
    string? RejectionReason,
    List<FileResponse> Files
);
