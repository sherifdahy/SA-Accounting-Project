using Microsoft.AspNetCore.Http;
using SA.Accounting.Application.Abstractions;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Infrastructure.Services;

public class AccountService(UserManager<ApplicationUser> userManager) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    public async Task<Result> ChangePasswordAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
    {
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var passwordResult = await _userManager.ResetPasswordAsync(user, token, password);

        if (!passwordResult.Succeeded)
        {
            var pwError = passwordResult.Errors.First();
            return Result.Failure(new Error(pwError.Code, pwError.Description, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }
}
