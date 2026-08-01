
using SA.Accounting.Application.Contracts.Auth.Responses;

namespace SA.Accounting.Application.Commands.Auth;

public record LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

