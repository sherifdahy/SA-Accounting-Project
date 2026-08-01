using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA.Accounting.Application.Contracts.Auth.Responses;

public record AuthResponse
{
    public int UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public IReadOnlyList<string> Roles { get; init; } = [];
    public IReadOnlyList<string> Permissions { get; init; } = [];
    public string AccessToken { get; init; } = string.Empty;
    public int ExpiresIn { get; init; }
}
