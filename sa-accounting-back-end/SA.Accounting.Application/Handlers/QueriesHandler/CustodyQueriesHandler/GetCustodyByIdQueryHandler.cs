using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.Custodies.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.Custody;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Services.Services;

namespace SA.Accounting.Application.Handlers.QueriesHandler.CustodyQueriesHandler;

public class GetCustodyByIdQueryHandler(IUnitOfWork unitOfWork,ICustodyBalanceCalculator custodyBalanceCalculator) : IRequestHandler<GetCustodyByIdQuery, Result<CustodyDetailsResponse>>
{
    private readonly ICustodyBalanceCalculator _balanceCalculator = custodyBalanceCalculator;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CustodyDetailsResponse>> Handle(GetCustodyByIdQuery query,CancellationToken cancellationToken)
    {
        var custody = await _unitOfWork.Custodies
            .FindAsync(x => x.Id == query.Id, [x=>x.Include(x=>x.User)] ,cancellationToken);

        if (custody is null)
            return Result.Failure<CustodyDetailsResponse>(CustodyErrors.NotFound);

        var balanceInfo = await _balanceCalculator.CalculateAsync(custody.Id, cancellationToken);

        var response = custody.Adapt<CustodyDetailsResponse>();

        return Result.Success(response);
    }
}
