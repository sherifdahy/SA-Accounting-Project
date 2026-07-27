using SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaims.Requests;

public record ExpenseClaimRequest (DateTime ClaimDate,string? Note,List<ExpenseClaimItemRequest> Items);
