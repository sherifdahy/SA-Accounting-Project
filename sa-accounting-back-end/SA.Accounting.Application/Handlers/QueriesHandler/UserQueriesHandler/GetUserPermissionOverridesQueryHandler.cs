using Mapster;
using Microsoft.AspNetCore.Identity;
using SA.Accounting.Application.Contracts.PermissionOverride.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Application.Queries.User;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.QueriesHandler.UserQueriesHandler;

public class GetUserPermissionOverridesQueryHandler(
    IUnitOfWork unitOfWork,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IRequestHandler<GetUserPermissionOverridesQuery, Result<UserPermissionOverridesResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<Result<UserPermissionOverridesResponse>> Handle(
        GetUserPermissionOverridesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            return Result.Failure<UserPermissionOverridesResponse>(UserErrors.NotFound);

        // =========================
        // 1. ROLE PERMISSIONS (Default)
        // =========================
        var roles = await _userManager.GetRolesAsync(user);

        var defaultPermissions = new HashSet<string>();

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) continue;

            var claims = await _roleManager.GetClaimsAsync(role);
            defaultPermissions.UnionWith(claims.Select(x => x.Value));
        }

        // =========================
        // 2. DENIED (Overrides)
        // =========================
        var overrides = await _unitOfWork.DeniedPermissions
            .FindAllAsync(x => x.UserId == request.UserId, cancellationToken);

        var denied = overrides
            .Select(x => x.Value)
            .ToList();

        return Result.Success(new UserPermissionOverridesResponse
        {
            Default = defaultPermissions.OrderBy(x => x).ToList(),
            Denied = denied.OrderBy(x => x).ToList()
        });
    }
}
