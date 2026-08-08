using Mapster;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Application.Commands.User;
using SA.Accounting.Application.Errors;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Interfaces;

public class UpdateUserCommandHandler(
    IIdentityService identityService,
    IUserService userService,
    IRoleService roleService,
    IUnitOfWork unitOfWork,
    IAccountService accountService,
    ICompanyService companyService) : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IUserService _userService = userService;
    private readonly IRoleService _roleService = roleService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAccountService _accountService = accountService;
    private readonly ICompanyService _companyService = companyService;

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        //Validation

        /// - validate user id
        if (await _userService.FindByIdAsync(request.UserId) is not ApplicationUser user)
            return Result.Failure(UserErrors.NotFound);

        /// - validate roles
        var rolesValidation = await _roleService.ValidateRolesAsync(request.Roles);

        if (rolesValidation.IsFailure)
            return rolesValidation;

        /// - validate email
        if (request.Email != user.Email)
        {
            var result = await _userService.ValidateEmailAsync(request.Email, cancellationToken);
            if(!result)    
                return Result.Failure(UserErrors.DuplicatedEmail);
        }

        /// - validate ssn
        if (!string.IsNullOrWhiteSpace(request.SSN) && request.SSN != user.SSN)
        {
            var result = await _userService.ValidateSSNAsync(request.SSN, cancellationToken);
            
            if(!result)
                return Result.Failure(UserErrors.DuplicateSSN);
        }

        /// - validate companyIds
        var validationCompaniesResult = await _companyService.ValidateCompaniesAsync(request.CompanyIds);

        if (validationCompaniesResult.IsFailure)
            return Result.Failure(CompanyErrors.NotFound);

        // mapping

        request.Adapt(user);

        user.UserName = request.Email;

        // open transaction

        await using var transaction =  await  _unitOfWork.BeginTransactionAsync(cancellationToken);

        // change password

        try
        {
            if (!string.IsNullOrEmpty(request.Password))
            {
                var changePasswordResult = await _accountService.ChangePasswordAsync(user, request.Password);

                if (changePasswordResult.IsFailure)
                    return Result.Failure(changePasswordResult.Error);
            }

            // update user

            var updateResult = await _userService.UpdateAsync(user);

            if (updateResult.IsFailure)
                return Result.Failure(updateResult.Error);

            // update roles

            var currentRoles = await _userService.GetRolesAsync(user, cancellationToken);

            var rolesToAdd = request.Roles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(request.Roles);

            if (rolesToAdd.Any())
            {
                await _userService.AddToRolesAsync(user, rolesToAdd, cancellationToken);
            }

            if (rolesToRemove.Any())
            {
                await _userService.RemoveFromRolesAsync(user, rolesToRemove, cancellationToken);
            }

            // update companies
            await _userService.UpdateAssignedCompaniesAsync(request.UserId, request.CompanyIds, cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return Result.Success();
        }
        catch 
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}