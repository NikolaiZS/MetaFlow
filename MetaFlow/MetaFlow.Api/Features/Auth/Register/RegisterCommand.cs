using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;

namespace MetaFlow.Api.Features.Auth.Register
{
    public record RegisterCommand(
    string Email,
    string Username,
    string Password,
    string? FullName
) : ICommand<AuthResponse>;
}