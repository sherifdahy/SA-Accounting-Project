using Mapster;
using SA.Accounting.Application.Contracts.ExpenseCategories.Responses;
using SA.Accounting.Application.Queries.ExpenseCategory;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.QueriesHandler.ExpenseCategoryQueriesHandler;

public class GetExpenseCategoriesHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetExpenseCategoriesQuery, Result<IReadOnlyList<ExpenseCategoryResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<IReadOnlyList<ExpenseCategoryResponse>>> Handle(GetExpenseCategoriesQuery query,CancellationToken cancellationToken)
    {
        var q = await _unitOfWork.ExpenseCategories.FindAllAsync(
            x => query.IncludeDisabled.HasValue && (query.IncludeDisabled == true || x.IsDisabled == false),
            cancellationToken);

        var response = q.Adapt<IReadOnlyList<ExpenseCategoryResponse>>();

        return Result.Success(response);
    }
}
