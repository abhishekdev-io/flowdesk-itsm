using FlowDesk.Api;
using FlowDesk.Api.Extensions;
using FlowDesk.Api.Middleware;
using FlowDesk.Infrastructure.Identity.Middleware;
using FlowDesk.Application.Interfaces;
using System.Text.Json.Serialization;
using Serilog;
using FlowDesk.Api.Hubs;
using Asp.Versioning;
using FlowDesk.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

// // Configure Serilog as the application's logging provider.
builder.Host.UseSerilog((context, services, config) => config
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/flowdesk-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        buffered: true,                  // Keeps it light on local disk I/O
        flushToDiskInterval: TimeSpan.FromSeconds(2))); // Flushes every 2 seconds

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApi(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// Setting up Api versioning
builder.Services
    .AddApiVersioning(options =>
    {
        options.ReportApiVersions = true;

        options.ApiVersionReader =
            new UrlSegmentApiVersionReader();
    })
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    })
    .AddOpenApi();

builder.Services.AddSignalR();

// Add health checks
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>(
        name: "postgres",
        tags: ["ready"]);

// Configure CORS
var flowDeskOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? ["https://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FlowDeskFrontend", policy =>
    {
        policy.WithOrigins(flowDeskOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure health check endpoints
// Liveness: checks whether the API is running.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

// Readiness: checks whether PostgreSQL is accessible.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Configure forwarded headers to handle reverse proxy scenarios.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};

// Allow Docker internal subnet proxying
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedHeadersOptions);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uncomment the following lines to enable HTTPS redirection in non-development environments
// if (!app.Environment.IsDevelopment())
// {
//     app.UseHttpsRedirection();
// }


// Apply pending EF Core migrations
await using (var scope = app.Services.CreateAsyncScope())
{
    try
    {
        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        Log.Information("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while applying database migrations.");
        throw;
    }
}

// Seed the database with initial data (Runs on both Dev and Production)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while running the automated database seeding engine.");
        throw;
    }
}

app.UseCors("FlowDeskFrontend");

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();
app.MapOpenApi().WithDocumentPerVersion();

app.MapHub<NotificationHub>("/api/v1/hubs/notifications");

try
{
    Log.Information("Starting FlowDesk API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "FlowDesk API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

