using SA.Accounting.Core.Entities.Attachments;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Platforms;
using SA.Accounting.Core.Interfaces;
using SA.Accounting.Infrastructure.Repository;

namespace SA.Accounting.Infrastructure.Presistance.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Companies = new Repository<Company>(_context);
        Owners = new Repository<Owner>(_context);
        Platforms = new Repository<Platform>(_context);
        Selectors = new Repository<Selector>(_context);
        ExpenseClaims = new Repository<ExpenseClaim>(_context);
        ExpenseCategories = new Repository<ExpenseCategory>(_context);
        ExpenseClaimItems = new Repository<ExpenseClaimItem>(_context);
        UserCompanies = new UserCompaniesRepository(_context);
        Accounts = new Repository<Account>(_context);
        DeniedPermissions = new Repository<UserRolePermissionOverride>(_context);
        Custodies = new Repository<Custody>(_context);
        CustodyMovements = new Repository<CustodyMovement>(_context);
        Attachments = new Repository<Attachment>(_context);
    }

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

    public void Dispose()
    {
        _context.Dispose();
    }

    public int Save()
    {
        return _context.SaveChanges();
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _context.Database.BeginTransactionAsync(cancellationToken);
    }
}
