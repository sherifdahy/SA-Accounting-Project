using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.Custodies.Responses;

public record CustodyDetailsResponse(
    int Id,
    string Number,
    bool IsDisabled,
    int UserId,
    string UserFullName,
    string? Note,
    DateTime CreatedAt,
    List<CustodyMovementResponse> Movements
);