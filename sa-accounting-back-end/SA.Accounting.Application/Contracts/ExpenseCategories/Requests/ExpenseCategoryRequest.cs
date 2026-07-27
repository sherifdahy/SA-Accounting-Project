using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.ExpenseCategories.Requests;

public class ExpenseCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public bool RequiresAttachment { get; set; }
    public bool IsDisabled { get; set; }
}
