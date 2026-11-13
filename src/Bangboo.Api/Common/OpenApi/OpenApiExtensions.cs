using Scalar.AspNetCore;

namespace Bangboo.Api.Common.OpenApi;

public static class OpenApiExtensions
{
    private const string DocumentUrl = "/openapi/v1.json";

    public static IServiceCollection AddApiDocs(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<CookieAuthDocumentTransformer>();
            options.AddOperationTransformer<AnonymousOperationTransformer>();
        });
        return services;
    }

    /// <summary>
    /// <c>/openapi/v1.json</c>, Scalar em <c>/scalar</c> e Swagger UI em <c>/swagger</c>.
    /// Ativo em Development ou com <c>Docs:Enabled=true</c>.
    /// </summary>
    public static WebApplication MapApiDocs(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment() && !app.Configuration.GetValue<bool>("Docs:Enabled"))
        {
            return app;
        }

        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference(options => options.WithTitle("Bangboo API")).AllowAnonymous();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(DocumentUrl, "Bangboo API");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}
