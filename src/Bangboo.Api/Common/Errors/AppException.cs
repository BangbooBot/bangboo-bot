namespace Bangboo.Api.Common.Errors;

/// <summary>
/// Base das exceções de domínio. Services lançam uma derivada; o
/// <see cref="GlobalExceptionHandler"/> converte em <see cref="ApiError"/>.
/// </summary>
public abstract class AppException(int status, string message) : Exception(message)
{
    public int Status { get; } = status;
}

public sealed class ValidationException(string message)
    : AppException(StatusCodes.Status400BadRequest, message);

public sealed class UnauthorizedException(string message = "Authentication required")
    : AppException(StatusCodes.Status401Unauthorized, message);

public sealed class ForbiddenException(string message = "You do not have permission to access this resource")
    : AppException(StatusCodes.Status403Forbidden, message);

public sealed class NotFoundException(string message)
    : AppException(StatusCodes.Status404NotFound, message);

public sealed class ConflictException(string message)
    : AppException(StatusCodes.Status409Conflict, message);

/// <summary>Falha em serviço externo (ex.: Discord fora do ar).</summary>
public sealed class ExternalServiceException(string message)
    : AppException(StatusCodes.Status502BadGateway, message);
