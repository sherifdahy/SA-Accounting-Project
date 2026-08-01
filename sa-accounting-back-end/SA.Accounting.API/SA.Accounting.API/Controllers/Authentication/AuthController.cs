using MediatR;
using Microsoft.AspNetCore.Mvc;
using SA.Accounting.Application.Commands.Auth;
using SA.Accounting.Application.Contracts.Auth.Requests;
using SA.Accounting.Core.Extensions;

namespace SA.Accounting.API.Controllers.Authentication;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand() with { Email = request.Email , Password = request.Password};
        var result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost(ConfirmEmailCommand.Route)]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost(ResendConfirmEmailCommand.Route)]
    public async Task<IActionResult> ResendConfirmEmail(ResendConfirmEmailCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost(ForgetPasswordCommand.Route)]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost(ResetPasswordCommand.Route)]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}
