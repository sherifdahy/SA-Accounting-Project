using SA.Accounting.Core.Entities.Companies;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Core.Entities.Relations;

public class UserCompany
{
    public int CompanyId { get; set; }
    public virtual Company Company { get; set; } = default!;
    public int UserId { get; set; }
}
