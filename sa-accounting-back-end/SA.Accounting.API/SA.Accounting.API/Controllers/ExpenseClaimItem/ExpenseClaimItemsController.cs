using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SA.Accounting.Application.Commands.ExpenseClaimItem;
using SA.Accounting.Application.Contracts.ExpenseClaimItems.Requests;
using SA.Accounting.Application.Queries.ExpenseClaimItem;
using SA.Accounting.Core.Extensions;

namespace SA.Accounting.API.Controllers.ExpenseClaimItem;

[Route("api/expense-claims")]
[ApiController]
[Authorize]
public class ExpenseClaimItemsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{claimId}/expense-claim-items")]
    public async Task<IActionResult> GetAll([FromRoute]int claimId,CancellationToken cancellationToken = default)
    {
        var query = new GetExpenseClaimItemsQuery(claimId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("expense-claim-items/{claimItemId}")]
    public async Task<IActionResult> GetById([FromRoute] int claimItemId, CancellationToken cancellationToken = default)
    {
        var query = new GetExpenseClaimItemQuery(claimItemId);
        var result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("{claimId}")]
    public async Task<IActionResult> Add([FromRoute] int claimId,[FromForm]ExpenseClaimItemRequest request, CancellationToken cancellationToken = default)
    {
        var command = new AddExpenseClaimItemCommand(claimId,request);
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(GetById),new { claimItemId = result.Value.Id },result.Value) : result.ToProblem();
    }

    [HttpPut("expense-claim-items/{claimItemId}")]
    public async Task<IActionResult> Update(int claimItemId, [FromForm] ExpenseClaimItemRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateExpenseClaimItemCommand(claimItemId, request);
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("expense-claim-items/{claimItemId}")]
    public async Task<IActionResult> Remove(int claimItemId,CancellationToken cancellationToken = default)
    {
        var command = new RemoveExpenseClaimItemCommand(claimItemId);
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
