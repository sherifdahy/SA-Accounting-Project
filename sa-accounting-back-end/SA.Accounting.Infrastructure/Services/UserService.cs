using Microsoft.AspNetCore.Http;
using SA.Accounting.Application.Abstractions;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Infrastructure.Presistance.Repository;

namespace SA.Accounting.Infrastructure.Services;

public class UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    public Task<ApplicationUser?> FindByEmailAsync(string email,CancellationToken cancellationToken = default)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public Task<ApplicationUser?> FindByIdAsync(int id,CancellationToken cancellationToken = default)
    {
        return _userManager.FindByIdAsync(id.ToString());
    }

    public Task<bool> ValidateSSNAsync(string ssn, CancellationToken cancellationToken = default)
    {
        return _userManager.Users.AnyAsync(x => x.SSN == ssn,cancellationToken);
    }

    public Task<bool> ValidateEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _userManager.Users.AnyAsync(x=>x.Email == email,cancellationToken);
    }

    public Task<IdentityResult> CreateAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
    {
        return _userManager.CreateAsync(user, password);
    }

    public async Task<Result> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
        }

        return Result.Success();

    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(ApplicationUser user,CancellationToken cancellationToken = default)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

    public Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        return _userManager.AddToRolesAsync(user, roles);
    }

    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        return _userManager.DeleteAsync(user);
    }

    public async Task<Result> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var result =  await _userManager.RemoveFromRolesAsync(user,roles);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code,error.Description,400));
    }
    public async Task AssignToCompaniesAsync(int userId, IEnumerable<int> companyIds, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.UserCompanies.AddRangeAsync(companyIds.Select(x =>
        {
            var userCompany = new UserCompany()
            {
                UserId = userId,
                CompanyId = x
            };
            return userCompany;
        }));

        await _unitOfWork.SaveAsync(cancellationToken);
    }

    public async Task UpdateAssignedCompaniesAsync(int userId,IEnumerable<int> companyIds,CancellationToken cancellationToken = default)
    {
        var newCompanyIds = companyIds.Distinct().ToList();

        var currentCompanies = await _unitOfWork.UserCompanies.FindAllAsync(
            x => x.UserId == userId,
            cancellationToken);

        var currentCompanyIds = currentCompanies
            .Select(x => x.CompanyId)
            .ToHashSet();

        var companiesToRemove = currentCompanies
            .Where(x => !newCompanyIds.Contains(x.CompanyId))
            .ToList();

        var companiesToAdd = newCompanyIds
            .Except(currentCompanyIds)
            .Select(companyId => new UserCompany
            {
                UserId = userId,
                CompanyId = companyId
            })
            .ToList();

        if (companiesToRemove.Count > 0)
        {
            _unitOfWork.UserCompanies.DeleteRange(companiesToRemove);
        }

        if (companiesToAdd.Count > 0)
        {
            await _unitOfWork.UserCompanies.AddRangeAsync(companiesToAdd, cancellationToken);
        }

        await _unitOfWork.SaveAsync(cancellationToken);
    }
}
