using Mapster;
using Microsoft.AspNetCore.Http;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Application.Commands.User;
using SA.Accounting.Application.Contracts.User.Responses;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Interfaces;

namespace SA.Accounting.Application.Handlers.CommandsHandler.UserCommandsHandler;

public class CreateUserCommandHandler(
    IIdentityService identityService,
    IUserService userService,
    IRoleService roleService,
    ICompanyService companyService,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, Result<UserResponse>>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IUserService _userService = userService;
    private readonly IRoleService _roleService = roleService;
    private readonly ICompanyService _companyService = companyService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UserResponse>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Validation

        if (await _userService.FindByEmailAsync(request.Email) is not null)
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        if (!string.IsNullOrWhiteSpace(request.SSN) &&
            await _userService.ValidateSSNAsync(request.SSN, cancellationToken))
        {
            return Result.Failure<UserResponse>(UserErrors.DuplicateSSN);
        }

        var roleValidation = await _roleService.ValidateRolesAsync(
            request.Roles,
            cancellationToken);

        if (roleValidation.IsFailure)
            return Result.Failure<UserResponse>(roleValidation.Error);

        var companyValidation = await _companyService.ValidateCompaniesAsync(
            request.CompanyIds,
            cancellationToken);

        if (companyValidation.IsFailure)
            return Result.Failure<UserResponse>(companyValidation.Error);

        // Transaction

        await using var transaction =
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var user = request.Adapt<ApplicationUser>();

            user.UserName = request.Email;

            var createUserResult = await _userService.CreateAsync(
                user,
                request.Password,
                cancellationToken);

            if (!createUserResult.Succeeded)
            {
                var error = createUserResult.Errors.First();

                await transaction.RollbackAsync(cancellationToken);

                return Result.Failure<UserResponse>(
                    new Error(
                        error.Code,
                        error.Description,
                        StatusCodes.Status400BadRequest));
            }

            var addRolesResult = await _userService.AddToRolesAsync(
                user,
                request.Roles);

            if (!addRolesResult.Succeeded)
            {
                var error = addRolesResult.Errors.First();

                await transaction.RollbackAsync(cancellationToken);

                return Result.Failure<UserResponse>(
                    new Error(
                        error.Code,
                        error.Description,
                        StatusCodes.Status400BadRequest));
            }

            // Assign Companies

            await _userService.AssignToCompaniesAsync(user.Id, request.CompanyIds, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success(user.Adapt<UserResponse>());
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}