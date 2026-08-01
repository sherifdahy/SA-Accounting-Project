using Mapster;
using SA.Accounting.Application.Commands.Account;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Entities.Relations;

namespace SA.Accounting.Application.Handlers.CommandsHandler.AccountCommandsHandler;

public class UpdateAccountCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateAccountCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Accounts.FindAsync(x => x.CompanyId == request.CompanyId && x.PlatformId == request.PlatfromId, cancellationToken) is not Account account)
            return Result.Failure(AccountErrors.NotFound);

        if (!_unitOfWork.Platforms.IsExist(x => x.Id == request.PlatfromId))
            return Result.Failure(PlatformErrors.NotFound);

        request.Adapt(account);

        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
