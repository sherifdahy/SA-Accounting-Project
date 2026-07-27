using Microsoft.AspNetCore.Http;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Commands.ExpenseClaimItem;

public record UpdateExpenseClaimItemCommand(int ClaimItemId, ExpenseClaimItemRequest Request) : IRequest<Result>;
