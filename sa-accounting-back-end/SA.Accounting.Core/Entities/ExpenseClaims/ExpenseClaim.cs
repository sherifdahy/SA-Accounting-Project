using SA.Accounting.Core.Entities.Base;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Enums;

namespace SA.Accounting.Core.Entities.ExpenseClaims;

public class ExpenseClaim : AuditableEntity
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime ClaimDate { get; set; }
    public string Note { get; set; } = string.Empty;
    public ExpenseClaimState CurrentState { get; set; } = ExpenseClaimState.Draft;
    public int UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ICollection<ExpenseClaimItem> Items { get; set; } = new HashSet<ExpenseClaimItem>();
    public virtual ICollection<ExpenseClaimHistory> Histories { get; set; } = new HashSet<ExpenseClaimHistory>();
    public virtual Movement? SettlementMovement { get; set; }
}
