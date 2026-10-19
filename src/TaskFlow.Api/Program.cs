using Serilog;
using TaskFlow.Application;
using TaskFlow.Infrastructure;
using TaskFlow.Api.ExceptionHandling;
using TaskFlow.Api.Middleware;
using TaskFlow.Api.Logging;


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
}


app.UseHttpsRedirection();

app.Run();

public partial class Program { }
