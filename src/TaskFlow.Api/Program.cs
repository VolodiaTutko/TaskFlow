using Serilog;
using TaskFlow.Application;
using TaskFlow.Infrastructure;
using TaskFlow.Api.ExceptionHandling;
using TaskFlow.Api.Middleware;
using TaskFlow.Api.Logging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;    


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services);
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

app.UseMiddleware<CorrelationIdMiddleware>();


app.UseSerilogRequestLogging(options =>
{
    SafeRequestLogging.Configure(options);

    options.Logger = app.Services
        .GetRequiredService<Serilog.ILogger>();
});



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}


app.UseHttpsRedirection();

app.MapHealthChecks("/health", new HealthCheckOptions   // without outside services
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions  // check Database
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

public partial class Program { }
