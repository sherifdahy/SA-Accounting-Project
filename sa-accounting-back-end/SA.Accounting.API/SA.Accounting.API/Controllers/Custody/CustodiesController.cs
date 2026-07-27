using MediatR;
using Microsoft.AspNetCore.Mvc;
using SA.Accounting.Application.Commands.Custody;
using SA.Accounting.Application.Contracts.Custodies.Requests;
using SA.Accounting.Application.Queries.Custody;
using SA.Accounting.Core.Extensions;

namespace SA.Accounting.API.Controllers.Custody;

[ApiController]
[Route("api/custodies")]
public class CustodiesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] int? userId = null, [FromQuery] bool? includeDisabled = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCustodiesQuery(userId, includeDisabled), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustodyById(int id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCustodyByIdQuery(id), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("{userId}")]
    public async Task<IActionResult> Create([FromRoute] int userId,[FromBody] CreateCustodyRequest request,CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new CreateCustodyCommand(userId,request), cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(GetCustodyById),new { id = result.Value.Id },result.Value) : result.ToProblem();
    }


    [HttpPatch("{id}/close")]
    public async Task<IActionResult> Close(int id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new CloseCustodyCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }


}