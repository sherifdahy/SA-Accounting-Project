using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Commands.ExpenseCategory;

public record ToggleStatusExpenseCategoryCommand(int Id) : IRequest<Result>;
