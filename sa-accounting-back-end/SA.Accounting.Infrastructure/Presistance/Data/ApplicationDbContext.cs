using Microsoft.AspNetCore.Http;
using SA.Accounting.Core.Entities.Attachments;
using SA.Accounting.Core.Entities.Base;
using SA.Accounting.Core.Entities.Companies;
using SA.Accounting.Core.Entities.Custodies;
using SA.Accounting.Core.Entities.ExpenseClaims;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Core.Entities.Platforms;
using System.Security.Claims;

namespace SA.Accounting.Infrastructure.Presistance.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,int>
{
    private readonly IHttpContextAccessor _httpContext;

    public ApplicationDbContext(IHttpContextAccessor context, DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _httpContext = context;
    }

    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<Account> Accounts { get; set; }
    public virtual DbSet<Owner> Owners { get; set; }
    public virtual DbSet<Platform> Platforms { get; set; }
    public virtual DbSet<Selector> Selectors { get; set; }
    public virtual DbSet<UserCompany> UserCompanies { get; set; }
    public virtual DbSet<UserRolePermissionOverride> DeniedPermissions { get; set; }

    public DbSet<Custody> Custodies { get; set; }
    public DbSet<CustodyMovement> CustodyMovements { get; set; }
    public DbSet<ExpenseClaim> ExpenseClaims { get; set; }
    public DbSet<ExpenseClaimItem> ExpenseClaimItems { get; set; }
    public DbSet<ExpenseClaimHistory> ExpenseClaimHistories { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<Attachment> Attachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
            .ToList()
            .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        if (entries.Any())
        {
            var currentUserId = GetUserId(_httpContext.HttpContext!.User);
            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Added)
                {
                    entityEntry.Property(x => x.CreatedById).CurrentValue = currentUserId;
                    entityEntry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entityEntry.Property(x => x.UpdatedById).CurrentValue = currentUserId;
                    entityEntry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    private static int GetUserId(ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        int.TryParse(userId, out var id);

        return id;
    }
}

