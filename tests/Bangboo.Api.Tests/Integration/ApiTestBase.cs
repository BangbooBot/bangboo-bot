using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bangboo.Api.Common.Errors;

namespace Bangboo.Api.Tests.Integration;

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<BangbooApiFactory>
{
    public const string Name = "api";
}

/// <summary>Base dos testes de integração: estado limpo a cada teste e helpers de asserção.</summary>
[Collection(ApiCollection.Name)]
public abstract class ApiTestBase(BangbooApiFactory factory) : IAsyncLifetime
{
    protected const string UserId = "111111111111111111";

    protected BangbooApiFactory Factory { get; } = factory;

    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    /// <summary>Mesmo JSON da API: camelCase e enums como texto (<c>"byTenure"</c>).</summary>
    protected static readonly JsonSerializerOptions Json = new(JsonSerializerOptions.Web)
    {
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public async ValueTask InitializeAsync() => await Factory.ResetAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <summary>Verifica o contrato de erro: status HTTP + corpo exatamente <c>{ status, message }</c>.</summary>
    protected static async Task<ApiError> AssertApiErrorAsync(HttpResponseMessage response, HttpStatusCode expected)
    {
        Assert.Equal(expected, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);

        using var json = await response.Content.ReadFromJsonAsync<JsonDocument>(Ct);
        Assert.NotNull(json);
        var properties = json.RootElement.EnumerateObject().Select(p => p.Name).Order().ToArray();
        Assert.Equal(["message", "status"], properties);

        var error = JsonSerializer.Deserialize<ApiError>(json.RootElement, JsonSerializerOptions.Web)!;
        Assert.Equal((int)expected, error.Status);
        Assert.False(string.IsNullOrWhiteSpace(error.Message));
        return error;
    }
}
