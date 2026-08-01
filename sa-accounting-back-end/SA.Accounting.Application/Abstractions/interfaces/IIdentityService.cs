using SA.Accounting.Application.Enums;
using SA.Accounting.Core.Entities.Identity;

namespace SA.Accounting.Application.Abstractions.interfaces;

public interface IIdentityService
{
    Task<ApplicationUser?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<LoginResult> ValidateLoginAsync(
        ApplicationUser user,
        string password,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolePermissionsAsync(
        string roleName,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetDeniedPermissionsAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default);
}
