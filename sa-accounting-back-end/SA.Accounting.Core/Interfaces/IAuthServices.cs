using Microsoft.AspNetCore.Identity;
using SA.Accounting.Core.Entities.Identity;

namespace SA.Accounting.Core.Interfaces;

public interface IAuthServices
{
    Task SendResetPasswordEmail(ApplicationUser applicationUser);
    Task SendConfirmationEmail(ApplicationUser applicationUser);
    Task GetTokenAsync(string email, string password);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string code, string newPassword);
}
