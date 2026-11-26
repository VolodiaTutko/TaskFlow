using TaskFlow.Application.DTOs.Auth;

namespace TaskFlow.Application.Abstractions.Authentification;

public interface IIdentityService
{
    Task<AuthResult> RegisterAsync(
        RegisterRequest request, CancellationToken cancellationToken);

    Task<AuthResult> LoginAsync(
        LoginRequest request, CancellationToken cancellationToken);
}