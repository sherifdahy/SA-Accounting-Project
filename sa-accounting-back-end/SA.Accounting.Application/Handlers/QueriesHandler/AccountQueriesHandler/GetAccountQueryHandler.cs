using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.Account.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.Account;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.QueriesHandler.AccountQueriesHandler;

public class GetAccountQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAccountQuery, Result<AccountResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<AccountResponse>> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await _unitOfWork.Accounts.FindAsync(
            x => x.PlatformId == request.PlatformId && x.CompanyId == request.CompanyId,
            cancellationToken,
            x => x.Platform);
        
        if (account == null)
            return Result.Failure<AccountResponse>(AccountErrors.NotFound);

        return Result.Success(account.Adapt<AccountResponse>());
    }
}
