using Microsoft.AspNetCore.Identity;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Relations;

namespace SA.Accounting.Core.Entities.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string Name { get; set; } = string.Empty;
    public string SSN { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public virtual ICollection<UserRolePermissionOverride> Permissions { get; set; } = [];
    public virtual ICollection<UserCompany> UserCompanies { get; set; } = new HashSet<UserCompany>();
    public virtual ICollection<Custody> Custodies { get; set; } = new HashSet<Custody>();
    public virtual ICollection<ExpenseClaim> ExpenseClaims { get; set; } = new HashSet<ExpenseClaim>();
    public virtual ICollection<ExpenseClaimHistory> ExpenseClaimHistories { get; set; } = new HashSet<ExpenseClaimHistory>();
}

