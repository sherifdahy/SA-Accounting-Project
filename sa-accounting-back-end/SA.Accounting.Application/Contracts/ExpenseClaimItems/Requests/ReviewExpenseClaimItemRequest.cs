using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;

public record ReviewExpenseClaimItemRequest(int ExpenseClaimItemId,ExpenseClaimItemState State,string? RejectionReason);
