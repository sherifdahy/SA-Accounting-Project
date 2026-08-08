using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Abstractions.interfaces;

public interface IRoleService
{
    Task<Result> ValidateRolesAsync(IReadOnlyCollection<string> roles, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetRolePermissionsAsync(string roleName,CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDeniedPermissionsAsync(ApplicationUser user,CancellationToken cancellationToken = default);
}
