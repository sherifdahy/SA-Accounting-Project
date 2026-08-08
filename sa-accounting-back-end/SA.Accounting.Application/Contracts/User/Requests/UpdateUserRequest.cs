namespace SA.Accounting.Application.Contracts.User.Requests;

public record UpdateUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? SSN { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = [];
    public List<int> CompanyIds { get; set; } = [];
}
