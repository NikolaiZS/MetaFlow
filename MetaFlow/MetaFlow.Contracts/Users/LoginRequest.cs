namespace MetaFlow.Contracts.Users
{
    public record LoginRequest(
    string EmailOrUsername,
    string Password
);
}