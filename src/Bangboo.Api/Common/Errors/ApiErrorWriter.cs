using Microsoft.AspNetCore.WebUtilities;

namespace Bangboo.Api.Common.Errors;

/// <summary>Escreve um <see cref="ApiError"/> diretamente na resposta (fora do MVC).</summary>
public static class ApiErrorWriter
{
    public static Task WriteAsync(HttpContext context, int status, string? message = null)
    {
        context.Response.StatusCode = status;
        var error = new ApiError(status, message ?? DefaultMessage(status));
        return context.Response.WriteAsJsonAsync(error, context.RequestAborted);
    }

    public static string DefaultMessage(int status) => status switch
    {
        StatusCodes.Status401Unauthorized => "Authentication required",
        StatusCodes.Status403Forbidden => "You do not have permission to access this resource",
        StatusCodes.Status404NotFound => "Resource not found",
        StatusCodes.Status500InternalServerError => "Internal server error",
        _ => ReasonPhrases.GetReasonPhrase(status) is { Length: > 0 } phrase ? phrase : "Unexpected error",
    };
}
