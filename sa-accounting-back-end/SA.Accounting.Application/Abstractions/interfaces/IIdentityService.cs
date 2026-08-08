using SA.Accounting.Application.Enums;
using SA.Accounting.Core.Entities.Identity;

namespace SA.Accounting.Application.Abstractions.interfaces;

public interface IIdentityService
{
    Task<LoginResult> ValidateLoginAsync(ApplicationUser user,string password,CancellationToken cancellationToken = default);
}
