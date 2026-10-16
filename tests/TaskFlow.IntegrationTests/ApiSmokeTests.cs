
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace TaskFlow.IntegrationTests;

public class ApiSmokeTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiSmokeTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UnknownEndpoint_ShouldReturn404()
    {
        var response = await _client.GetAsync(
            "/api/non-existing-endpoint");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}
