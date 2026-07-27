using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Contracts.Custodies.Responses;

public record CustodyResponse(
    int Id,
    string Number,
    int UserId,
    string UserFullName,
    bool IsDisabled,
    decimal Balance,
    DateTime CreatedAt
);
