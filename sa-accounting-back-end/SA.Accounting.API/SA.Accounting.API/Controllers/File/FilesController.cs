using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SA.Accounting.Application.Queries.File;
using SA.Accounting.Core.Extensions;

namespace SA.Accounting.API.Controllers.File;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FilesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Download(Guid id,CancellationToken cancellationToken = default)
    {
        var query = new GetFileQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? File(result.Value.fileContent, result.Value.contentType, result.Value.fileName) : result.ToProblem();
    }
}
