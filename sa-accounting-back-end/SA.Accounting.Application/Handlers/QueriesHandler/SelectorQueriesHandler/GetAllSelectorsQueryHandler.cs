using Mapster;
using SA.Accounting.Application.Contracts.Selector.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.Selector;
using SA.Accounting.Core.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Handlers.QueriesHandler.SelectorQueriesHandler;

public class GetAllSelectorsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllSelectorsQuery, Result<List<SelectorResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<List<SelectorResponse>>> Handle(GetAllSelectorsQuery request, CancellationToken cancellationToken)
    {
        if (!_unitOfWork.Platforms.IsExist(x => x.Id == request.PlatformId))
            return Result.Failure<List<SelectorResponse>>(PlatformErrors.NotFound);

        var selectors = await _unitOfWork.Selectors.FindAllAsync(
            x => x.PlatformId == request.PlatformId && (request.IncludeDisabled == true || x.IsDeleted == false),
            cancellationToken);

        return Result.Success(selectors.Adapt<List<SelectorResponse>>());
    }
}
