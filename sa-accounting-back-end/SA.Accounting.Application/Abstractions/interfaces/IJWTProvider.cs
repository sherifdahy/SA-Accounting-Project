using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Abstractions.interfaces;

public interface IJWTProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser applicationUser, IEnumerable<string> applicationRoles, IEnumerable<string> permissions);
    int? ValidateToken(string token);
}
