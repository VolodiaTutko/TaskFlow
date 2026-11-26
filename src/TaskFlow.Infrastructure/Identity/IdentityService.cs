using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Abstractions.Authentification;
using TaskFlow.Application.DTOs.Auth;

namespace TaskFlow.Infrastructure.Identity;

internal sealed class IdentityService(
    UserManager<ApplicationUser> userManager) : IIdentityService
{
    public async Task<AuthResult> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return AuthResult.Success();
        }

        if (result.Errors.Any(e =>
                e.Code is nameof(IdentityErrorDescriber.DuplicateEmail)
                       or nameof(IdentityErrorDescriber.DuplicateUserName)))
        {
            return AuthResult.Failure(
                AuthError.EmailAlreadyExists,
                "A user with this email already exists.");
        }

        return AuthResult.Failure(
            AuthError.ValidationFailed,
            result.Errors.Select(e => e.Description).ToArray());
    }

    public async Task<AuthResult> LoginAsync(
        LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return InvalidCredentials();
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            return AuthResult.Failure(
                AuthError.LockedOut,
                "Account is temporarily locked. Try again later.");
        }

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            await userManager.AccessFailedAsync(user);
            return InvalidCredentials();
        }

        await userManager.ResetAccessFailedCountAsync(user);
        return AuthResult.Success();
    }

    private static AuthResult InvalidCredentials() =>
        AuthResult.Failure(
            AuthError.InvalidCredentials,
            "Invalid email or password.");
}