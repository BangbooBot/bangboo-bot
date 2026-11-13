using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Bangboo.Api.Common.OpenApi;

/// <summary>Remove o requisito de cookie das operações marcadas com <c>[AllowAnonymous]</c>.</summary>
public sealed class AnonymousOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken ct)
    {
        var isAnonymous = context.Description.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any();
        if (isAnonymous)
        {
            operation.Security = [];
        }

        return Task.CompletedTask;
    }
}
