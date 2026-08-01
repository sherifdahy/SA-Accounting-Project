namespace SA.Accounting.Infrastructure.Identity.Entities;

public class UserRolePermissionOverride
{
    public string Value { get; set; } = string.Empty;
    public int UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
}   
