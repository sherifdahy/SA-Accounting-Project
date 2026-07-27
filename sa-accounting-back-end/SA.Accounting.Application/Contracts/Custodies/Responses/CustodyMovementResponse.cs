using SA.Accounting.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.Custodies.Responses;

public record CustodyMovementResponse(
    int Id,
    int CustodyId,
    MovementType Type,
    decimal Amount,
    string? Note,
    int? ExpenseClaimId,
    DateTime CreatedAt
);