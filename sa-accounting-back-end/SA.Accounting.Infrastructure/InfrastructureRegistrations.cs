using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Core.Entities.Identity;
using SA.Accounting.Infrastructure.Presistance.Data;
using SA.Accounting.Infrastructure.Presistance.Repository;
using SA.Accounting.Infrastructure.Services;

namespace SA.Accounting.Infrastructure;
public static class InfrastructureRegistrations
{
    public static void AddInfrastructureRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddDbContextConfig(configuration.GetConnectionString("default")!);
        services.AddIdentityConfig();
        services.AddServicesConfig();
    }

    private static void AddIdentityConfig(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser,ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Lockout
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // Password
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            // SignIn
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;

            // User
            //options.User.AllowedUserNameCharacters = "abcdefghijklmnopq";
            options.User.RequireUniqueEmail = true;

            // Cookie Settings for MVC
        });

    }
    private static void AddDbContextConfig(this IServiceCollection services,string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(x =>
        {
            x.UseSqlServer(connectionString);
        });
    }
    private static void AddServicesConfig(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IIdentityService, IdentityService>();
    }
}

