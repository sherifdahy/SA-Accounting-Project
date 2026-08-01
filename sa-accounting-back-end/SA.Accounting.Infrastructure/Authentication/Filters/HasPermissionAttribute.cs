using Microsoft.AspNetCore.Authorization;

namespace SA.Accounting.Infrastructure.Authentication.Filters;
public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}
