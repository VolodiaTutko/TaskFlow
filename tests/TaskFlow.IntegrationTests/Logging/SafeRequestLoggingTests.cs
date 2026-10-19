using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace TaskFlow.IntegrationTests.Logging;

public sealed class SafeRequestLoggingTests
{
    [Fact]
    public async Task RequestLogging_ShouldNotExposeQuerySecrets()
    {
        var sink = new InMemorySink();

        using var factory = new WebApplicationFactory<Program>()
           .WithWebHostBuilder(builder =>
           {
               builder.UseEnvironment("Testing");

               builder.ConfigureServices(services =>
               {
                   services.AddSerilog((_, configuration) =>
                   {
                       configuration
                           .MinimumLevel.Information()
                           .Enrich.FromLogContext()
                           .WriteTo.Sink(sink);
                   });
               });
           });

        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/openapi/v1.json?access_token=TEST_SECRET_123");

        Assert.Equal(
            System.Net.HttpStatusCode.NotFound,
            response.StatusCode);

        Assert.True(
            response.Headers.TryGetValues(
                "X-Correlation-ID",
                out var correlationIds));

        var correlationId = Assert.Single(correlationIds);

        Assert.False(string.IsNullOrWhiteSpace(correlationId));

        var requestLogs = sink.Events
            .Where(e => e.Properties.ContainsKey("RequestMethod")
                     && e.Properties.ContainsKey("StatusCode"))
            .ToList();

        Assert.True(
        sink.Events.Count > 0,
        "InMemorySink received no Serilog events.");

            var eventDetails = string.Join(
                Environment.NewLine,
                sink.Events.Select(e =>
                    $"Message: {e.RenderMessage()}; " +
                    $"Properties: {string.Join(", ", e.Properties.Keys)}"));

        Assert.True(
            requestLogs.Count > 0,
            $"No HTTP request logs found. Events: {eventDetails}");

        foreach (var logEvent in requestLogs)
        {
            var logText = logEvent.RenderMessage();

            Assert.DoesNotContain(
                "TEST_SECRET_123",
                logText);

            Assert.DoesNotContain(
                "TEST_SECRET_123",
                logEvent.ToString());

            Assert.True(
                logEvent.Properties.ContainsKey("CorrelationId"));
        }
    }
}
