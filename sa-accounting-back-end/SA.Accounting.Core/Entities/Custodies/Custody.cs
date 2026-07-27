using SA.Accounting.Core.Entities.Base;
using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Core.Entities.Custodies;

public class Custody : AuditableEntity
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public bool IsDisabled { get; set; } = false;
    public string? Note { get; set; }
    public int UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = default!;
    public virtual ICollection<CustodyMovement> Movements { get; set; } = new HashSet<CustodyMovement>();
}
