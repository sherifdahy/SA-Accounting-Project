using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Application.Commands.Auth;
using SA.Accounting.Application.Contracts.Auth.Responses;
using SA.Accounting.Application.Enums;
using SA.Accounting.Application.Errors;

namespace SA.Accounting.Application.Handlers.CommandsHandler.AuthCommandsHandler;

public class LoginCommandHandler(IUserService userService,IRoleService roleService,IIdentityService identityService,IJWTProvider accessTokenService) : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserService _userService = userService;
    private readonly IRoleService _roleService = roleService;
    private readonly IIdentityService _identityService = identityService;
    private readonly IJWTProvider _accessTokenService = accessTokenService;
    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userService.FindByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var result = await _identityService.ValidateLoginAsync(user, request.Password, cancellationToken);
        
        if(result == LoginResult.LockedOut)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);
        else if(result == LoginResult.EmailNotConfirmed)
            return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);
        else if(result == LoginResult.Disabled)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        else if(result == LoginResult.InvalidCredentials)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var roles = await _userService.GetRolesAsync(user, cancellationToken);

        var permissions = new List<string>();

        foreach (var roleName in roles)
        {
            var rolePermissions = await _roleService.GetRolePermissionsAsync(roleName, cancellationToken);

            permissions.AddRange(rolePermissions);
        }

        var deniedPermissions = await _roleService.GetDeniedPermissionsAsync(user, cancellationToken);
        permissions = permissions
            .Except(deniedPermissions)
            .Distinct()
            .ToList();

        var accessTokenData = _accessTokenService.GenerateToken(user, roles.ToList(), permissions);

        return Result.Success(new AuthResponse()
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email!,
            Roles = roles,
            Permissions = permissions,
            AccessToken = accessTokenData.token,
            ExpiresIn = accessTokenData.expiresIn
        });
    }
}

