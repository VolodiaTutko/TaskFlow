using TaskFlow.Application.Abstractions.Authentification;
using TaskFlow.Application.DTOs.Auth;
using TaskFlow.Application.Validation;

namespace TaskFlow.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            IIdentityService identityService,
            CancellationToken cancellationToken) =>
        {
            var errors = AuthRequestValidator.Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var result = await identityService.RegisterAsync(
                request, cancellationToken);

            return result.Succeeded
                ? Results.StatusCode(StatusCodes.Status201Created)
                : ToProblem(result);
        });

        group.MapPost("/login", async (
            LoginRequest request,
            IIdentityService identityService,
            CancellationToken cancellationToken) =>
        {
            var errors = AuthRequestValidator.Validate(request);
            if (errors.Count > 0)
            {
                return Results.ValidationProblem(errors);
            }

            var result = await identityService.LoginAsync(
                request, cancellationToken);

            return result.Succeeded
                ? Results.Ok()
                : ToProblem(result);
        });

        return app;
    }

    private static IResult ToProblem(AuthResult result) => result.Error switch
    {
        AuthError.EmailAlreadyExists => Results.Problem(
            statusCode: StatusCodes.Status409Conflict,
            title: "Conflict",
            detail: result.Messages.FirstOrDefault()),

        AuthError.InvalidCredentials => Results.Problem(
            statusCode: StatusCodes.Status401Unauthorized,
            title: "Unauthorized",
            detail: result.Messages.FirstOrDefault()),

        AuthError.LockedOut => Results.Problem(
            statusCode: StatusCodes.Status429TooManyRequests,
            title: "Too Many Requests",
            detail: result.Messages.FirstOrDefault()),

        AuthError.ValidationFailed => Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                ["password"] = result.Messages.ToArray()
            }),

        _ => Results.Problem()
    };
}