using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SA.Accounting.API;

public static class GlobalExeptionHandler
{
    public static void HandleExeption(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(o =>
        {
            o.Run(async context =>
            {
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();

                var exception = errorFeature.Error;

                if(!(exception is FluentValidation.ValidationException validationException))
                {
                    var problemDetails = new ProblemDetails()
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Internal Server Error",
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    };

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(problemDetails);

                    return;
                }


                var errors = validationException.Errors.Select(error => new
                {
                    Property = error.PropertyName,
                    Message = error.ErrorMessage
                });

                var errorContent = JsonConvert.SerializeObject(errors);

                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(errorContent);
            });
        });
    }
}
