using Mapster;
using Microsoft.EntityFrameworkCore;
using SA.Accounting.Application.Contracts.Platform.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.Platform;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.QueriesHandler.PlatformQueriesHandler;

public class GetPlatformQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetPlatformQuery, Result<PlatformDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    async Task<Result<PlatformDetailResponse>> IRequestHandler<GetPlatformQuery, Result<PlatformDetailResponse>>.Handle(GetPlatformQuery request, CancellationToken cancellationToken)
    {
        var platform = await _unitOfWork.Platforms.FindAsync(
            x => x.Id == request.Id,
            cancellationToken,
            x => x.Selectors);

        if (platform == null)
            return Result.Failure<PlatformDetailResponse>(PlatformErrors.NotFound);

        return Result.Success(platform.Adapt<PlatformDetailResponse>());
    }
}
