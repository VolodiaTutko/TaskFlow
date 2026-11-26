using System.Net.Mail;
using TaskFlow.Application.DTOs.Auth;

namespace TaskFlow.Application.Validation;

public static class AuthRequestValidator
{
    public static Dictionary<string, string[]> Validate(RegisterRequest request)
    {
        var errors = ValidateEmail(request.Email);

        if (string.IsNullOrEmpty(request.Password))
        {
            errors["password"] = ["Password is required."];
        }

        if (request.DisplayName is { Length: > 100 })
        {
            errors["displayName"] = ["Display name must be 100 characters or fewer."];
        }

        return errors;
    }

    public static Dictionary<string, string[]> Validate(LoginRequest request)
    {
        var errors = ValidateEmail(request.Email);

        if (string.IsNullOrEmpty(request.Password))
        {
            errors["password"] = ["Password is required."];
        }

        return errors;
    }

    private static Dictionary<string, string[]> ValidateEmail(string? email)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(email))
        {
            errors["email"] = ["Email is required."];
        }
        else if (email.Length > 256
                 || !MailAddress.TryCreate(email, out var address)
                 || address.Address != email)
        {
            errors["email"] = ["Email is not valid."];
        }

        return errors;
    }
}