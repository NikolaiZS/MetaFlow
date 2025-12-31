namespace MetaFlow.Contracts.Users
{
    public record RegisterRequest(
    string Email,
    string Username,
    string Password,
    string? FullName
);
}