using FluentValidation.AspNetCore;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;

using SA.Accounting.Application.Behaviors;

namespace SA.Accounting.Application;

public static class ApplicationRegistrations
{
    public static void AddApplicationRegistrations(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddMediatR(o =>
        {
            o.RegisterServicesFromAssembly(typeof(ApplicationRegistrations).Assembly);
            o.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddFluentValidationAutoValidation().AddValidatorsFromAssembly(typeof(ApplicationRegistrations).Assembly);

        var mappingConfiguration = TypeAdapterConfig.GlobalSettings;
        mappingConfiguration.Scan(typeof(ApplicationRegistrations).Assembly);
        services.AddSingleton<IMapper>(new Mapper(mappingConfiguration));

    }
}
