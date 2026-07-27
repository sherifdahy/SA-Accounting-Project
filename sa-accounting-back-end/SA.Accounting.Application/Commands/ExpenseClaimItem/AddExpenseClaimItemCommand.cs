using Microsoft.AspNetCore.Http;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;

namespace SA.Accounting.Application.Commands.ExpenseClaimItem;

public record AddExpenseClaimItemCommand(int ClaimId,ExpenseClaimItemRequest Request) : IRequest<Result<ExpenseClaimItemResponse>>;