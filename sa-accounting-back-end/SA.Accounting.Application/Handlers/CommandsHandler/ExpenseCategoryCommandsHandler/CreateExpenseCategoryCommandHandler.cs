using Mapster;
using SA.Accounting.Application.Commands.ExpenseCategory;
using SA.Accounting.Application.Contracts.ExpenseCategories.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseCategoryCommandHandler;

public class CreateExpenseCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateExpenseCategoryCommand, Result<ExpenseCategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result<ExpenseCategoryResponse>> Handle(CreateExpenseCategoryCommand command,CancellationToken cancellationToken)
    {
        var exists = _unitOfWork.ExpenseCategories
            .IsExist(x => x.Name == command.Request.Name);

        if (exists)
            return Result.Failure<ExpenseCategoryResponse>(ExpenseCategoryErrors.DuplicateName);

        var entity = command.Request.Adapt<ExpenseCategory>();

        await _unitOfWork.ExpenseCategories.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success(entity.Adapt<ExpenseCategoryResponse>());
    }
}
