using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Commands.User;

public record UpdateUserCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? SSN { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = [];
    public List<int> CompanyIds { get; set; } = [];

}
