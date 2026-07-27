using SA.Accounting.Application.Commands.ExpenseCategory;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.ExpenseCategoryCommandHandler;

public class ToggleStatusExpenseCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ToggleStatusExpenseCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public async Task<Result> Handle(ToggleStatusExpenseCategoryCommand command,CancellationToken cancellationToken)
    {
        var entity = await _unitOfWork.ExpenseCategories
            .FindAsync(x => x.Id == command.Id,[],cancellationToken);

        if (entity is null)
            return Result.Failure(ExpenseCategoryErrors.NotFound);

        entity.IsDisabled = !entity.IsDisabled;
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
