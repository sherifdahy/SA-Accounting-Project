using SA.Accounting.Core.Entities.Attachments;
using SA.Accounting.Core.Entities.Base;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Relations;

namespace SA.Accounting.Core.Entities.Companies;

public class Company : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TaxRegistrationNumber { get; set; } = string.Empty;
    public string TaxFileNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public virtual ICollection<Owner> Owners { get; set; } = new HashSet<Owner>();
    public virtual ICollection<Account> Accounts { get; set; } = new HashSet<Account>();
    public virtual ICollection<UserCompany> UserCompanies { get; set; } = new HashSet<UserCompany>();
    public virtual ICollection<ExpenseClaimItem> ExpenseClaimItems { get; set; } = new HashSet<ExpenseClaimItem>();
    public virtual ICollection<Attachment> Attachments { get; set; } = new HashSet<Attachment>();
}
