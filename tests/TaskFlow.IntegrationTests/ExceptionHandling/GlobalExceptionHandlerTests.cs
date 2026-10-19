using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TaskFlow.IntegrationTests.ExceptionHandling;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task UnhandledException_ShouldReturnProblemDetails()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddTransient<IStartupFilter,
                        ThrowingEndpointStartupFilter>();
                });
            });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/test-error");

        // Assert
        Assert.Equal(
            HttpStatusCode.InternalServerError,
            response.StatusCode);

        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content
            .ReadFromJsonAsync<ProblemDetails>();

        Assert.NotNull(problem);
        Assert.Equal(500, problem.Status);
        Assert.Equal("Internal Server Error", problem.Title);
        Assert.Equal(
            "An unexpected error occurred.",
            problem.Detail);

        Assert.DoesNotContain(
            "Sensitive internal error",
            await response.Content.ReadAsStringAsync());

        Assert.True(
            problem.Extensions.TryGetValue("traceId", out var traceId));

        Assert.NotNull(traceId);

        Assert.False(
            string.IsNullOrWhiteSpace(traceId.ToString()));

    }

    private sealed class ThrowingEndpointStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(
            Action<IApplicationBuilder> next)
        {
            return app =>
            {
                next(app);

                app.Use(async (context, nextMiddleware) =>
                {
                    if (context.Request.Path == "/test-error")
                    {
                        throw new InvalidOperationException(
                            "Sensitive internal error");
                    }

                    await nextMiddleware(context);
                });
            };
        }
    }
}
