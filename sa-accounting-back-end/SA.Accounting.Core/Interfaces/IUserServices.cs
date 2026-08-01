using Microsoft.AspNetCore.Identity;
using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Core.Interfaces;

public interface IUserServices
{
    Task<ApplicationUser> GetProfileAsync(int userId, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateProfileAsync(ApplicationUser applicationUser, CancellationToken cancellationToken = default);
    Task<IdentityResult> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
}

