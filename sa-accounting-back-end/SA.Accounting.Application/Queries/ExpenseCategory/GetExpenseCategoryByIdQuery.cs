using SA.Accounting.Application.Contracts.ExpenseCategories.Responses;

namespace SA.Accounting.Application.Queries.ExpenseCategory;

public record GetExpenseCategoryByIdQuery(int Id): IRequest<Result<ExpenseCategoryResponse>>;
