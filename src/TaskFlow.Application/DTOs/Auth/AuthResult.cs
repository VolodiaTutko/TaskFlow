namespace TaskFlow.Application.DTOs.Auth;

public enum AuthError
{
    None,
    InvalidCredentials,
    EmailAlreadyExists,
    ValidationFailed,
    LockedOut
}

public sealed record AuthResult(
    bool Succeeded,
    AuthError Error,
    IReadOnlyList<string> Messages)
{
    public static AuthResult Success() =>
        new(true, AuthError.None, []);

    public static AuthResult Failure(
        AuthError error, params string[] messages) =>
        new(false, error, messages);
}