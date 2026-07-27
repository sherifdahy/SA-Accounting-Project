using Mapster;
using SA.Accounting.Application.Contracts.Custodies.Responses;
using SA.Accounting.Application.Queries.Custody;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Application.Handlers.QueriesHandler.CustodyQueriesHandler;

public class GetCustodiesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCustodiesQuery, Result<IReadOnlyList<CustodyResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<IReadOnlyList<CustodyResponse>>> Handle(GetCustodiesQuery query,CancellationToken cancellationToken)
    {
        var entities = await _unitOfWork.Custodies.FindAllAsync(x=>
                            (!query.UserId.HasValue || x.UserId == query.UserId.Value) &&
                            query.IncludeDisabled.HasValue && (query.IncludeDisabled == true || x.IsDisabled == false), [],cancellationToken);

        var response = entities.Adapt<IReadOnlyList<CustodyResponse>>();

        return Result.Success(response);
    }
}
