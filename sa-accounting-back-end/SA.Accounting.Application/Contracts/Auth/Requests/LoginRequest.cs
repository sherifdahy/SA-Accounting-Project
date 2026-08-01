using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SA.Accounting.Application.Contracts.Auth.Requests;

public record LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
