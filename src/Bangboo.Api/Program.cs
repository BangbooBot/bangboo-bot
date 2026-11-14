using System.Text.Json.Serialization;
using Bangboo.Api.Common;
using Bangboo.Api.Common.Auth;
using Bangboo.Api.Common.Discord;
using Bangboo.Api.Common.Errors;
using Bangboo.Api.Common.OpenApi;
using Bangboo.Api.Extensions;
using Bangboo.Core.Data;
using Bangboo.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Infra
builder.Services.AddBangbooDatabase(builder.Configuration);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddApiErrorHandling();
builder.Services.AddBangbooAuth(builder.Environment);
builder.Services.AddDiscordApi();
builder.Services.AddApiDocs();
builder.Services.AddControllers(options => options.Filters.Add(new ProducesAttribute("application/json")))
    .AddJsonOptions(options => options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict);
// Números estritos também para o gerador do OpenAPI: o front recebe `number`, não `number | string`.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict);

// Services da aplicação
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseApiErrorHandling();
app.MapApiDocs();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Durante a geração do OpenAPI no build não há banco: nunca migrar nesse modo.
if (!BuildContext.IsGeneratingOpenApi && app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
}

await app.RunAsync();

/// <summary>Exposto para os testes de integração (WebApplicationFactory).</summary>
public partial class Program;
