using SA.Accounting.Application.Contracts.User.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Commands.User;

public record CreateUserCommand : IRequest<Result<UserResponse>>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? SSN { get; set; }
    public string? PhoneNumber { get; set; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public IReadOnlyList<int> CompanyIds { get; init; } = [];
}
