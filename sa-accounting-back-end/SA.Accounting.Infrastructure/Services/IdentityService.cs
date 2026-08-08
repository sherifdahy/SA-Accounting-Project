using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Application.Enums;
using SA.Accounting.Core.Abstractions.Consts;
using SA.Accounting.Core.Entities.Identity;

namespace SA.Accounting.Infrastructure.Services;

public class IdentityService(UserManager<ApplicationUser> userManager) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
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

   
}
