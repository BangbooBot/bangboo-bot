using System.Text.Json;
using Bangboo.Api.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bangboo.Api.Tests.Unit;

public sealed class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _handler = new(NullLogger<GlobalExceptionHandler>.Instance);

    [Fact]
    public async Task TryHandleAsync_AppException_KeepsStatusAndMessage()
    {
        var (status, error) = await HandleAsync(new ConflictException("Already exists"));

        Assert.Equal(409, status);
        Assert.Equal(new ApiError(409, "Already exists"), error);
    }

    [Fact]
    public async Task TryHandleAsync_UnknownException_ReturnsGeneric500()
    {
        var (status, error) = await HandleAsync(new InvalidOperationException("secret detail"));

        Assert.Equal(500, status);
        Assert.Equal(new ApiError(500, "Internal server error"), error);
    }

    private async Task<(int Status, ApiError? Error)> HandleAsync(Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var handled = await _handler.TryHandleAsync(context, exception, TestContext.Current.CancellationToken);
        Assert.True(handled);

        context.Response.Body.Position = 0;
        var error = await JsonSerializer.DeserializeAsync<ApiError>(
            context.Response.Body,
            JsonSerializerOptions.Web,
            TestContext.Current.CancellationToken);

        return (context.Response.StatusCode, error);
    }
}
