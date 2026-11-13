using Bangboo.Api.Common.Auth;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Bangboo.Api.Common.OpenApi;

/// <summary>Título/descrição do documento e o esquema de segurança do cookie de sessão.</summary>
public sealed class CookieAuthDocumentTransformer : IOpenApiDocumentTransformer
{
    private const string SchemeId = "cookie";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken ct)
    {
        document.Info = new OpenApiInfo
        {
            Title = "Bangboo API",
            Version = "v1",
            Description =
                "API do dashboard do Bangboo. Erros sempre retornam `{ status, message }`. " +
                "A autenticação é feita por cookie (`bangboo.auth`) criado em `GET /api/auth/login`.",
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeId] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Cookie,
            Name = AuthExtensions.CookieName,
            Description = "Cookie de sessão criado após o login com Discord.",
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeId, document)] = [],
        });

        return Task.CompletedTask;
    }
}
