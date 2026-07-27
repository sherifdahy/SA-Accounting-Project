using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.Extensions.Options;
using SA.Accounting.API;
using Scalar.AspNetCore;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
});

builder.AddDepenecyInjectionRegistration();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.AddPreferredSecuritySchemes("Bearer");
});


app.UseHangfireDashboard("/jobs", new DashboardOptions()
{
    Authorization = [
        new HangfireCustomBasicAuthenticationFilter(){
            User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
            Pass = app.Configuration.GetValue<string>("HangfireSettings:Password"),
        },
    ],
    DashboardTitle = "SA Accounting",
});

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.HandleExeption();
app.MapStaticAssets();
app.Run();
