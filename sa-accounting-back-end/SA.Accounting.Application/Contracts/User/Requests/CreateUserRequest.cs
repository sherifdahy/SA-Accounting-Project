namespace SA.Accounting.Application.Contracts.User.Requests;

public record CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? SSN { get; set; }
    public string? PhoneNumber { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
    public IReadOnlyList<int> CompanyIds { get; set; } = [];
}
