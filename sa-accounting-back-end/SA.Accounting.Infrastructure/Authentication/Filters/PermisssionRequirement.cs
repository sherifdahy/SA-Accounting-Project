using Microsoft.AspNetCore.Authorization;

namespace SA.Accounting.Infrastructure.Authentication.Filters;
public class PermisssionRequirement : IAuthorizationRequirement
{
    public string permission { get; }
    public PermisssionRequirement(string permission)
    {
        this.permission = permission;
    }


}
