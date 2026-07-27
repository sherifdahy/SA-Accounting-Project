using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseClaims.Responses;

public record CreateExpenseClaimResponse(int Id,string Number,DateTime CreatedAt,DateTime? UpdatedAt);