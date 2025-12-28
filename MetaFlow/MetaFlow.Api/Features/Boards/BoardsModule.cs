using Carter;
using MediatR;
using MetaFlow.Api.Features.Boards.CreateBoard;
using MetaFlow.Api.Features.Boards.DeleteBoard;
using MetaFlow.Api.Features.Boards.GetBoard;
using MetaFlow.Api.Features.Boards.GetBoards;
using MetaFlow.Api.Features.Boards.UpdateBoard;
using MetaFlow.Contracts.Boards;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MetaFlow.Api.Features.Boards
{
    public class BoardsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/boards")
                .WithTags("Boards")
                .RequireAuthorization();

            group.MapPost("", async (
                CreateBoardRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new CreateBoardCommand(
                    request.Name,
                    request.Description,
                    request.MethodologyPresetId,
                    request.IsPublic,
                    request.IsTemplate,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("CreateBoard")
            .Produces<BoardResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("", async (
                [FromQuery] bool includeArchived,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetBoardsQuery(userId, includeArchived);
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetBoards")
            .Produces<List<BoardListResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapGet("{boardId:guid}", async (
                Guid boardId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetBoardQuery(boardId, userId);
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetBoard")
            .Produces<BoardResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapPatch("{boardId:guid}", async (
                Guid boardId,
                UpdateBoardRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateBoardCommand(
                    boardId,
                    userId,
                    request.Name,
                    request.Description,
                    request.IsPublic,
                    request.IsArchived
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("UpdateBoard")
            .Produces<BoardResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            group.MapDelete("{boardId:guid}", async (
                Guid boardId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new DeleteBoardCommand(boardId, userId);
                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("DeleteBoard")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }

}
