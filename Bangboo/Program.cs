using System.Text.Json;
using System.Text.Json.Serialization;
using bangboo_backend.Data;
using Bangboo;
using Bangboo.Server.Services;
using Bangboo.Utils.Tools;
using Microsoft.EntityFrameworkCore;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using Scalar.AspNetCore;

// Web API
var builder = WebApplication.CreateBuilder(args);

// Dicord bot
var discordBuilder = Host.CreateDefaultBuilder()
    .UseDiscordGateway(options =>
    {
        options.Intents = GatewayIntents.All;
    })
    .ConfigureServices(services =>
    {
        services.AddGatewayHandlers(typeof(Program).Assembly);
        services.AddScoped<AutomodService>();
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DbConnection")
            );
        });

        services.AddScoped<DatabaseService>();
    })
    .UseApplicationCommands();

var bot = discordBuilder.Build();

bot.AddModules(typeof(Program).Assembly);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard", policy =>
    {
        /*
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
        */
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddOptions<Env>()
    .Bind(builder.Configuration.GetSection("Env"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.Strict;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DbConnection")
    );
});
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<DiscordService>(_ => new DiscordService(bot));
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
    });

    app.UseReDoc(options =>
    {
        options.SpecUrl("/openapi/v1.json");
    });

    app.MapScalarApiReference();
}

app.UseCors("AllowDashboard");

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (context.Database.CanConnect())
    {
        Console.WriteLine("PostgreSQL connected!!");
    }
    else
    {
        Console.WriteLine("PostgreSQL connection failed!");
    }
}

await Task.WhenAll(app.RunAsync(), bot.RunAsync());
