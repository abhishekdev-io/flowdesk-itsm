using FlowDesk.Api;
using FlowDesk.Api.Extensions;
using FlowDesk.Api.Middleware;
using FlowDesk.Infrastructure.Identity.Middleware;
using FlowDesk.Application.Interfaces;
using System.Text.Json.Serialization;
using Serilog;

Log.Logger = new LoggerConfiguration() 
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();
    
var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    using var scope = app.Services.CreateScope();
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

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();//initialise serilog
app.UseAuthentication();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

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
    Log.CloseAndFlush(); // ensures buffered log entries are actually written before process exit
}

