using Microsoft.AspNetCore.Mvc;

namespace Bangboo.Api.Common.Errors;

public static class ErrorHandlingExtensions
{
    /// <summary>Garante que todo erro saia como <see cref="ApiError"/>.</summary>
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            // Sem ProblemDetails automático: o contrato é sempre { status, message }.
            options.SuppressMapClientErrors = true;
            options.InvalidModelStateResponseFactory = context =>
            {
                var message = context.ModelState.Values
                    .SelectMany(entry => entry.Errors)
                    .Select(error => error.ErrorMessage)
                    .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text))
                    ?? "Invalid request";

                return new BadRequestObjectResult(new ApiError(StatusCodes.Status400BadRequest, message));
            };
        });

        return services;
    }

    public static IApplicationBuilder UseApiErrorHandling(this IApplicationBuilder app)
    {
        // O delegate só roda se nenhum IExceptionHandler tratar a exceção (não deve acontecer).
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = context => ApiErrorWriter.WriteAsync(context, StatusCodes.Status500InternalServerError),
        });

        // Respostas de erro sem corpo (404 de rota, 405, 401/403 da autorização...) viram ApiError.
        app.UseStatusCodePages(context =>
            ApiErrorWriter.WriteAsync(context.HttpContext, context.HttpContext.Response.StatusCode));

        return app;
    }
}
