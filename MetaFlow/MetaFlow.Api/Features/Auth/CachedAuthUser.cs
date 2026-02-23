namespace MetaFlow.Api.Features.Auth;

public record CachedAuthUser(
    Guid Id,
    string Email,
    string Username,
    string? FullName,
    string? AvatarUrl,
    string? PasswordHash,
    bool EmailVerified,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);

