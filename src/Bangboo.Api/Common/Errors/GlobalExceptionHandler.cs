using Microsoft.AspNetCore.Diagnostics;

namespace Bangboo.Api.Common.Errors;

/// <summary>
/// Converte qualquer exceção não tratada em <see cref="ApiError"/>.
/// <see cref="AppException"/> mantém status e mensagem; o resto vira 500 genérico (e é logado).
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is AppException appException)
        {
            await ApiErrorWriter.WriteAsync(context, appException.Status, appException.Message);
            return true;
        }

        logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        await ApiErrorWriter.WriteAsync(context, StatusCodes.Status500InternalServerError);
        return true;
    }
}
