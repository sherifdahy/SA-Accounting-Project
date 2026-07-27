using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Queries.ExpenseClaimItem;

public record GetExpenseClaimItemQuery(int Id) : IRequest<Result<ExpenseClaimItemResponse>>;
