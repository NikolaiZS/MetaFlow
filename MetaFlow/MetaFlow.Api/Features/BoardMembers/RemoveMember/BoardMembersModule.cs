using Carter;
using MediatR;
using MetaFlow.Api.Features.BoardMembers.GetBoardMembers;
using MetaFlow.Api.Features.BoardMembers.InviteMember;
using MetaFlow.Api.Features.BoardMembers.RemoveMember;
using MetaFlow.Api.Features.BoardMembers.UpdateMemberRole;
using MetaFlow.Contracts.BoardMembers;
using System.Security.Claims;

namespace MetaFlow.Api.Features.BoardMembers.RemoveMember
{
    public class BoardMembersModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/boards/{boardId:guid}/members")
                .WithTags("Board Members")
                .RequireAuthorization();

            group.MapPost("", async (
                Guid boardId,
                InviteMemberRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new InviteMemberCommand(
                    boardId,
                    request.UserId,
                    request.Role,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("InviteMember")
            .Produces<BoardMemberResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("", async (
                Guid boardId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetBoardMembersQuery(boardId, userId);
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetBoardMembers")
            .Produces<List<BoardMemberResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPatch("{memberId:guid}", async (
                Guid boardId,
                Guid memberId,
                UpdateMemberRoleRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateMemberRoleCommand(
                    boardId,
                    memberId,
                    request.Role,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("UpdateMemberRole")
            .Produces<BoardMemberResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapDelete("{memberId:guid}", async (
                Guid boardId,
                Guid memberId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new RemoveMemberCommand(boardId, memberId, userId);
                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("RemoveMember")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
