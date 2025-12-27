using Carter;
using MediatR;
using MetaFlow.Api.Features.Users.GetCurrentUser;
using MetaFlow.Contracts.Users;
using Microsoft.AspNetCore.Authorization;


namespace MetaFlow.Api.Features.Users
{
    public class UsersModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/users")
                .WithTags("Users");

            group.MapGet("me", [Authorize] async (ISender sender) =>
            {
                var query = new GetCurrentUserQuery();
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetCurrentUser")
            .RequireAuthorization()
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }

}
