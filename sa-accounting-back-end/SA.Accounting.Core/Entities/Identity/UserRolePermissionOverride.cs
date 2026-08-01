namespace SA.Accounting.Core.Entities.Identity;

public class UserRolePermissionOverride
{
    public string Value { get; set; } = string.Empty;
    public int UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
}
