using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Bangboo.Api.Common.OpenApi;

/// <summary>
/// Normaliza quebras de linha das descrições (vindas dos comentários XML) para <c>\n</c>.
/// Sem isso, o <c>openapi/Bangboo.Api.json</c> gerado no Windows (CRLF) difere do gerado
/// no Linux/CI (LF) e o check de contrato do CI falha.
/// </summary>
public sealed class LineEndingsTransformer : IOpenApiOperationTransformer, IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken ct)
    {
        operation.Summary = Normalize(operation.Summary);
        operation.Description = Normalize(operation.Description);

        foreach (var parameter in operation.Parameters ?? [])
        {
            if (parameter is OpenApiParameter concrete)
            {
                concrete.Description = Normalize(concrete.Description);
            }
        }

        if (operation.Responses is not null)
        {
            foreach (var response in operation.Responses.Values)
            {
                if (response is OpenApiResponse concrete)
                {
                    concrete.Description = Normalize(concrete.Description);
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken ct)
    {
        schema.Description = Normalize(schema.Description);
        return Task.CompletedTask;
    }

    private static string? Normalize(string? text) => text?.Replace("\r\n", "\n");
}
