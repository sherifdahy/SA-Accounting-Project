using SA.Accounting.Core.Entities.Attachments;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Platforms;
using SA.Accounting.Core.Entities.Relations;
using SA.Accounting.Core.Interfaces;

namespace SA.Accounting.Core.Entities.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IRepository<Company> Companies { get; }
    public IRepository<Owner> Owners { get; }
    public IRepository<Platform> Platforms { get; }
    public IRepository<Selector> Selectors { get; }
    public IRepository<ExpenseClaim> ExpenseClaims { get; }
    public IRepository<ExpenseCategory> ExpenseCategories { get; }
    public IRepository<ExpenseClaimItem> ExpenseClaimItems { get; }
    public IUserCompaniesRepository UserCompanies { get; }
    public IRepository<Account> Accounts { get; }
    public IRepository<UserRolePermissionOverride> DeniedPermissions { get; }
    public IRepository<Custody> Custodies { get; }
    public IRepository<CustodyMovement> CustodyMovements { get; }
    public IRepository<Attachment> Attachments { get; }
    int Save();
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
}

