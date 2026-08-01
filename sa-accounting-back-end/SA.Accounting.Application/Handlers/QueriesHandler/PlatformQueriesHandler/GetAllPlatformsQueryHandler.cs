using Mapster;
using SA.Accounting.Application.Contracts.Platform.Responses;
using SA.Accounting.Application.Queries.Platform;
using SA.Accounting.Core.Abstractions;
using SA.Accounting.Core.Entities.Interfaces;
using SA.Accounting.Core.Entities.Platforms;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SA.Accounting.Application.Handlers.QueriesHandler.PlatformQueriesHandler;

public class GetAllPlatformsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllPlatformsQuery, Result<List<PlatformResponse>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<List<PlatformResponse>>> Handle(GetAllPlatformsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Platform, bool>> query = x => (request.IncludeDisabled == true || x.IsDeleted == false);

        var platforms = await _unitOfWork.Platforms.FindAllAsync(query, cancellationToken);

        return Result.Success(platforms.Adapt<List<PlatformResponse>>());
    }
}
