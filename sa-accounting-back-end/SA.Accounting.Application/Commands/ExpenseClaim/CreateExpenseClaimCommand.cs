using SA.Accounting.Application.Contracts.ExpenseClaims.Requests;
using SA.Accounting.Application.Contracts.ExpenseClaims.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Commands.ExpenseClaim;

public record CreateExpenseClaimCommand(int UserId) : IRequest<Result<CreateExpenseClaimResponse>>;
