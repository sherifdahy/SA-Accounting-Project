using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;

public record ExpenseClaimItemRequest
(
    int CompanyId,
    int ExpenseCategoryId,
    string? Note,
    decimal Amount,
    IFormFileCollection? Files
);
