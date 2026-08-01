using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using SA.Accounting.Application;
using SA.Accounting.Infrastructure;
using SA.Accounting.Services;
using System.Text;
using SA.Accounting.Application.Abstractions.interfaces;
using SA.Accounting.Infrastructure.Authentication;
using SA.Accounting.Infrastructure.Authentication.Filters;

namespace SA.Accounting.API;

public static class DependencyInjection
{
    public static void AddDepenecyInjectionRegistration(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationRegistrations(builder.Configuration);
        builder.Services.AddServicesRegistrations(builder.Configuration);
        builder.Services.AddInfrastructureRegistrations(builder.Configuration);
        builder.Services.AddCorsConfig(builder.Configuration);
        builder.Services.AddAuthConfig(builder.Configuration); 
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScelerConfig();
    }

    private static IServiceCollection AddScelerConfig(this IServiceCollection services)
    {
        services.AddSingleton<BearerSecuritySchemeTransformer>();

        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }

    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJWTProvider, JWTProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration("Jwt")
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            // validation 
            o.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateActor = true,
                ValidateLifetime = true,

                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Key)),
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
    private static IServiceCollection AddCorsConfig(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
            });
        });
        return services;
    }

}
