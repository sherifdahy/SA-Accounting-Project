using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Enums;

public enum LoginResult
{
    Success,
    InvalidCredentials,
    LockedOut,
    EmailNotConfirmed,
    Disabled
}
