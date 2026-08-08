using Microsoft.AspNetCore.Identity;
using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SA.Accounting.Application.Abstractions.interfaces;
public interface IUserService
{
    Task<ApplicationUser?> FindByEmailAsync(string email,CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByIdAsync(int id,CancellationToken cancellationToken = default);
    Task<IdentityResult> CreateAsync(ApplicationUser user,string password,CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<IdentityResult> DeleteAsync(ApplicationUser user,CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default);
    Task<IdentityResult> AddToRolesAsync(ApplicationUser user,IEnumerable<string> roles,CancellationToken cancellationToken = default);
    Task<Result> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    
    Task<bool> ValidateSSNAsync(string ssn,CancellationToken cancellationToken = default);
    Task<bool> ValidateEmailAsync(string email,CancellationToken cancellationToken = default);
    
    Task AssignToCompaniesAsync(int userId, IEnumerable<int> companyIds, CancellationToken cancellationToken = default);
    Task UpdateAssignedCompaniesAsync(int userId,IEnumerable<int> companyIds,CancellationToken cancellationToken = default);
}
