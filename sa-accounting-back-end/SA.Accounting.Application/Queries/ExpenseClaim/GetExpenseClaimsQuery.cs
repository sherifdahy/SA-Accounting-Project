using SA.Accounting.Application.Contracts.ExpenseClaimItems.Responses;
using SA.Accounting.Application.Contracts.ExpenseClaims.Responses;

namespace SA.Accounting.Application.Queries.ExpenseClaim;

public record GetExpenseClaimsQuery : IRequest<Result<IReadOnlyList<ExpenseClaimSummaryResponse>>>;
