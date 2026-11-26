using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskFlow.Application.Abstractions.Authentification;
using TaskFlow.Application.DTOs.Auth;

namespace TaskFlow.IntegrationTests.Auth;

public sealed class AuthEndpointsTests
{
    private static readonly object ValidRegister =
        new { email = "user@example.com", password = "Test1234", displayName = "User" };

    private static readonly object ValidLogin =
        new { email = "user@example.com", password = "Test1234" };

    [Fact]
    public async Task Register_ShouldReturn201_WhenSucceeded()
    {
        var response = await PostAsync(
            "/api/auth/register", ValidRegister, AuthResult.Success());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldReturn409_WhenEmailExists()
    {
        var response = await PostAsync(
            "/api/auth/register", ValidRegister,
            AuthResult.Failure(AuthError.EmailAlreadyExists, "exists"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldReturn400_WhenPasswordRejectedByIdentity()
    {
        var response = await PostAsync(
            "/api/auth/register", ValidRegister,
            AuthResult.Failure(AuthError.ValidationFailed, "Password too short."));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            "application/problem+json",
            response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Register_ShouldReturn400_WhenEmailIsInvalid_BeforeCallingService()
    {
        // The fake would return success, so 400 proves the validator ran first.
        var response = await PostAsync(
            "/api/auth/register",
            new { email = "not-an-email", password = "Test1234" },
            AuthResult.Success());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturn200_WhenSucceeded()
    {
        var response = await PostAsync(
            "/api/auth/login", ValidLogin, AuthResult.Success());

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenCredentialsAreInvalid()
    {
        var response = await PostAsync(
            "/api/auth/login", ValidLogin,
            AuthResult.Failure(AuthError.InvalidCredentials, "invalid"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturn429_WhenAccountIsLockedOut()
    {
        var response = await PostAsync(
            "/api/auth/login", ValidLogin,
            AuthResult.Failure(AuthError.LockedOut, "locked"));

        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturn400_WhenPasswordIsEmpty()
    {
        var response = await PostAsync(
            "/api/auth/login",
            new { email = "user@example.com", password = "" },
            AuthResult.Success());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static async Task<HttpResponseMessage> PostAsync(
        string url, object body, AuthResult serviceResult)
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<IIdentityService>();
                    services.AddScoped<IIdentityService>(
                        _ => new FakeIdentityService(serviceResult));
                });
            });

        using var client = factory.CreateClient();
        return await client.PostAsJsonAsync(url, body);
    }

    private sealed class FakeIdentityService(AuthResult result) : IIdentityService
    {
        public Task<AuthResult> RegisterAsync(
            RegisterRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(result);

        public Task<AuthResult> LoginAsync(
            LoginRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(result);
    }
}