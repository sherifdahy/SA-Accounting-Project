using Mapster;
using SA.Accounting.Application.Contracts.ExpenseCategories.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.ExpenseCategory;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Infrastructure.Presistance.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.QueriesHandler.ExpenseCategoryQueriesHandler;

public class GetExpenseCategoryByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetExpenseCategoryByIdQuery, Result<ExpenseCategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<ExpenseCategoryResponse>> Handle(GetExpenseCategoryByIdQuery query,CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.ExpenseCategories
            .GetByIdAsync(query.Id,cancellationToken);

        if (entity is null)
            return Result.Failure<ExpenseCategoryResponse>(ExpenseCategoryErrors.NotFound);

        return Result.Success(entity.Adapt<ExpenseCategoryResponse>());
    }
}
