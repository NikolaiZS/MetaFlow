namespace MetaFlow.Contracts.Users
{
    public record UserResponse(
    Guid Id,
    string Email,
    string Username,
    string? FullName,
    string? AvatarUrl,
    bool EmailVerified,
    DateTime CreatedAt
);
}