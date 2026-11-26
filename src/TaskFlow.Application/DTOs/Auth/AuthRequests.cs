namespace TaskFlow.Application.DTOs.Auth;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string? DisplayName);

public sealed record LoginRequest(
    string Email,
    string Password);