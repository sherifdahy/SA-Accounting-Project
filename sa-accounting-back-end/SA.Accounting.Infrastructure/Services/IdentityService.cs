using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Application.Enums;
using SA.Accounting.Core.Abstractions.Consts;
using SA.Accounting.Core.Entities.Identity;

namespace SA.Accounting.Infrastructure.Services;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IUnitOfWork unitOfWork) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task<ApplicationUser?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public async Task<LoginResult> ValidateLoginAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.IsLockedOutAsync(user))
            return LoginResult.LockedOut;

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);
            return await _userManager.IsLockedOutAsync(user)
                ? LoginResult.LockedOut
                : LoginResult.InvalidCredentials;
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        if (user.IsDisabled)
            return LoginResult.Disabled;

        if (!await _userManager.IsEmailConfirmedAsync(user))
            return LoginResult.EmailNotConfirmed;

        return LoginResult.Success;
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return roles.ToList();
    }

    public async Task<IReadOnlyList<string>> GetRolePermissionsAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
            return [];

        var claims = await _roleManager.GetClaimsAsync(role);

        return claims
            .Where(claim => claim.Type == Permissions.Type)
            .Select(claim => claim.Value)
            .Distinct()
            .ToList();
    }

    public async Task<IReadOnlyList<string>> GetDeniedPermissionsAsync(
        ApplicationUser user,
        CancellationToken cancellationToken = default)
    {
        var deniedPermissions = await _unitOfWork.DeniedPermissions.FindAllAsync(
            permission => permission.UserId == user.Id,
            cancellationToken);

        return deniedPermissions
            .Select(permission => permission.Value)
            .Distinct()
            .ToList();
    }


}
