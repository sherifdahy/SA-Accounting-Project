using SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaims.Requests;


public record ReviewExpenseClaimRequest(string? Note, List<ReviewExpenseClaimItemRequest> Items);

