using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Abstractions.interfaces;

public interface IAccountService
{
    Task<Result> ChangePasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default);
}
