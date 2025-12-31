namespace MetaFlow.Contracts.Users
{
    public record AuthResponse(
    Guid UserId,
    string Email,
    string Username,
    string? FullName,
    string Token,
    DateTime ExpiresAt
);
}