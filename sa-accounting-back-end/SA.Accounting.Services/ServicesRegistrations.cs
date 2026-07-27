using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SA.Accounting.Core.Interfaces;
using SA.Accounting.Services.OptionClasses;
using SA.Accounting.Services.Services;

namespace SA.Accounting.Services;

public static class ServicesRegistrations
{
    public static void AddServicesRegistrations(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddScoped<IEmailSender, EmailServices>();
        services.AddScoped<IFileService, FileService>();

        services.AddScoped<ICustodyNumberGenerator, CustodyNumberGenerator>();
        services.AddScoped<ICustodyBalanceCalculator, CustodyBalanceCalculator>();
        services.AddScoped<IExpenseClaimNumberGenerator, ExpenseClaimNumberGenerator>();
        services
            .AddOptions<MailSettings>()
            .BindConfiguration(nameof(MailSettings))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Only add Hangfire if not running migrations
        if (!IsRunningMigrations())
        {
            services.AddHandfireConfig(configuration);
        }

        services.AddScoped<IAuthServices,AuthServices>();
        services.AddScoped<IUserServices, UserServices>();
    }

    private static bool IsRunningMigrations()
    {
        var args = Environment.GetCommandLineArgs();
        return args.Contains("migrations") || args.Contains("--no-build");
    }

    private static void AddHandfireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer();
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
            config.UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"));
        });
    }
}
