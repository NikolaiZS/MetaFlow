using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Users;

namespace MetaFlow.Api.Features.Auth.Login
{
    public record LoginCommand(
    string EmailOrUsername,
    string Password
) : ICommand<AuthResponse>;
}