using SA.Accounting.Core.Entities.Base;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Core.Entities.ExpenseClaims;

public class ExpenseClaimHistory : AuditableEntity
{
    public int Id { get; set; }
    public int ExpenseClaimId { get; set; }
    public virtual ExpenseClaim ExpenseClaim { get; set; } = default!;
    public ExpenseClaimState FromState { get; set; }
    public ExpenseClaimState ToState { get; set; }
    public string? Note { get; set; }
}
