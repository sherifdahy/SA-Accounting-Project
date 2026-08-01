using Microsoft.AspNetCore.Identity;
using SA.Accounting.Application.Commands.User;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Abstractions.Consts;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.UserCommandsHandler;

public class UpdatePermissionOverridesCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdatePermissionOverridesCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(
        UpdatePermissionOverridesCommand request, CancellationToken cancellationToken)
    {
        // =========================
        // 1. VALIDATE USER
        // =========================
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        // =========================
        // 2. VALIDATE PERMISSIONS
        // =========================
        var invalidPermissions = request.DeniedPermissions
            .Except(Permissions.GetAllPermissions());

        if (invalidPermissions.Any())
            return Result.Failure(PermissionErrors.InvalidPermission);

        // =========================
        // 3. REMOVE OLD OVERRIDES
        // =========================
        var existingOverrides = await _unitOfWork.DeniedPermissions
            .FindAllAsync(x => x.UserId == request.UserId, cancellationToken);

        if (existingOverrides.Count() > 0)
            _unitOfWork.DeniedPermissions.DeleteRange(existingOverrides);

        // =========================
        // 4. ADD NEW OVERRIDES
        // =========================
        var newOverrides = request.DeniedPermissions
            .Distinct()
            .Select(permission => new UserRolePermissionOverride
            {
                UserId = request.UserId,
                Value = permission
            })
            .ToList();

        if (newOverrides.Count > 0)
            await _unitOfWork.DeniedPermissions.AddRangeAsync(newOverrides);

        // =========================
        // 5. SAVE
        // =========================
        await _unitOfWork.SaveAsync(cancellationToken);

        return Result.Success();
    }
}
