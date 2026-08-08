using SA.Accounting.Application.Abstractions;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Core.Abstractions.Consts;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Infrastructure.Presistance.Repository;

namespace SA.Accounting.Infrastructure.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager,UserManager<ApplicationUser> userManager,IUnitOfWork unitOfWork) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> ValidateRolesAsync(IReadOnlyCollection<string> roles,CancellationToken cancellationToken = default)
    {
        var existingRoles = await _roleManager.Roles
            .Select(x => x.Name!)
            .ToListAsync(cancellationToken);

        var missingRoles = roles.Except(existingRoles).ToList();

        if (missingRoles.Any())
        {
            return Result.Failure(new Error("Roles.NotFound",$"The following roles do not exist: {string.Join(", ", missingRoles)}",404));
        }

        return Result.Success();
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
