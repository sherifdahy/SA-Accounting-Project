using SA.Accounting.Application.Commands.Account;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.AccountCommandsHandler;

public class ToggleStatusAccountCommandHander(IUnitOfWork unitOfWork) : IRequestHandler<ToggleStatusAccountCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ToggleStatusAccountCommand request, CancellationToken cancellationToken)
    {
        var email = await _unitOfWork.Accounts.FindAsync(x => x.CompanyId == request.CompanyId && x.PlatformId == request.PlatformId, cancellationToken);

        if (email == null)
            return Result.Failure(AccountErrors.NotFound);

        email.IsDeleted = !email.IsDeleted;

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
