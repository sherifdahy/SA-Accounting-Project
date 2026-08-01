using SA.Accounting.Application.Commands.ExpenseCategory;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseCategoryCommandHandler;

public class DeleteExpenseCategoryCommandHandler (IUnitOfWork unitOfWork): IRequestHandler<DeleteExpenseCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(DeleteExpenseCategoryCommand command,CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.ExpenseCategories
            .FindAsync(x => x.Id == command.Id, cancellationToken);

        if (entity is null)
            return Result.Failure(ExpenseCategoryErrors.NotFound);

        var isUsed = _unitOfWork.ExpenseClaimItems
            .IsExist(x => x.ExpenseCategoryId == command.Id);

        if (isUsed)
            return Result.Failure(ExpenseCategoryErrors.InUse);

        _unitOfWork.ExpenseCategories.Delete(entity);
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
