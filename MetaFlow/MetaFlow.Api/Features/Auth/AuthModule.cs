using Carter;
using MediatR;
using MetaFlow.Api.Features.Auth.Login;
using MetaFlow.Api.Features.Auth.Register;
using MetaFlow.Contracts.Users;

namespace MetaFlow.Api.Features.Auth
{
    public class AuthModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/auth")
                .WithTags("Authentication");

            group.MapPost("register", async (RegisterRequest request, ISender sender) =>
            {
                var command = new RegisterCommand(
                    request.Email,
                    request.Username,
                    request.Password,
                    request.FullName
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("Register")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            group.MapPost("login", async (LoginRequest request, ISender sender) =>
            {
                var command = new LoginCommand(
                    request.EmailOrUsername,
                    request.Password
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("Login")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }

}
