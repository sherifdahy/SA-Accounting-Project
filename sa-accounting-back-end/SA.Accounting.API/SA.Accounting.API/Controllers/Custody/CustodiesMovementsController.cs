using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SA.Accounting.Application.Commands.Custody;
using SA.Accounting.Application.Contracts.Custodies.Requests;
using SA.Accounting.Application.Queries.Custody;
using SA.Accounting.Core.Extensions;

namespace SA.Accounting.API.Controllers.Custody;

[Route("api/custodies/{custodyId}/movements")]
[ApiController]
public class CustodiesMovementsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet()]
    public async Task<IActionResult> GetMovements(int custodyId, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCustodyMovementsByCustodyIdQuery(custodyId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost()]
    public async Task<IActionResult> CreateMovement(int custodyId, [FromBody] CreateMovementRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCustodyMovementCommand(custodyId, request), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
