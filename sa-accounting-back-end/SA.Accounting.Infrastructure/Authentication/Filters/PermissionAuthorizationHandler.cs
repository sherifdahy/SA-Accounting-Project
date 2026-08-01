using Microsoft.AspNetCore.Authorization;
using SA.Accounting.Core.Abstractions.Consts;

namespace SA.Accounting.Infrastructure.Authentication.Filters;
public class PermissionAuthorizationHandler : AuthorizationHandler<PermisssionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermisssionRequirement requirement)
    {
        var user = context.User.Identity;

        if (user is null || !user.IsAuthenticated)
            return;

        var hasPermission = context.User.Claims.Any(x => x.Value == requirement.permission && x.Type == Permissions.Type);

        if(hasPermission) context.Succeed(requirement);
    }

}
