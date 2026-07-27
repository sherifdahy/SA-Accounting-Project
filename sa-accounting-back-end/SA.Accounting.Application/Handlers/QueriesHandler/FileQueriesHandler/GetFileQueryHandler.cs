using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.File;
using SA.Accounting.Core.Interfaces;

namespace SA.Accounting.Application.Handlers.QueriesHandler.FileCommandsHandler;

public class GetFileQueryHandler(IFileService fileService) : IRequestHandler<GetFileQuery, Result<(byte[] fileContent, string contentType, string fileName)>>
{
    private readonly IFileService _fileService = fileService;
    public async Task<Result<(byte[] fileContent, string contentType, string fileName)>> Handle(GetFileQuery request, CancellationToken cancellationToken)
    {
        var result = await _fileService.DownloadAsync(request.Id, cancellationToken);

        if (result.fileContent is [])
            return Result.Failure<(byte[] fileContent, string contentType, string fileName)>(FileErrors.NotFound);

        return Result.Success((result.fileContent, result.contentType, result.fileName));
    }
}
