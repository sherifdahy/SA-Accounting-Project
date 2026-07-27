using SA.Accounting.Core.Entities.Base;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.ExpenseClaims;

namespace SA.Accounting.Core.Entities.Attachments;

public class Attachment : AuditableEntity
{
    public int Id { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string? Note { get; set; }

    public int CompanyId { get; set; }
    public virtual Company Company { get; set; } = default!;

    public int? ExpenseClaimItemId { get; set; }
    public virtual ExpenseClaimItem? ExpenseClaimItem { get; set; }
}
