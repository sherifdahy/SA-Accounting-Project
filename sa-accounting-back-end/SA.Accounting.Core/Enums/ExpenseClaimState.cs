using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SA.Accounting.Core.Enums;

public enum ExpenseClaimState
{
    Draft = 1,             // الموظف لسه بيكتب
    Submitted = 2,         // الموظف بعت للمراجعة
    Approved = 3,          // كل البنود اتوافقت
    Rejected = 4,          // كله اترفض
    Settled = 5,           // اتخصم من العهدة
    Cancelled = 6,         // اتلغى
    ReturnedForEdit = 7    // رجع للموظف يعدل
}