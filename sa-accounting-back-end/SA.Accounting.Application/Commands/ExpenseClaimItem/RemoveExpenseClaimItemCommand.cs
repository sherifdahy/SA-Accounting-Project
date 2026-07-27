using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Commands.ExpenseClaimItem;

public record RemoveExpenseClaimItemCommand(int ClaimItemId) : IRequest<Result>;