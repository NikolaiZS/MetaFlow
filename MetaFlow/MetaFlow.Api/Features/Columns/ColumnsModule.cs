using Carter;
using MediatR;
using MetaFlow.Api.Features.Columns.CreateColumns;
using MetaFlow.Api.Features.Columns.DeleteColumn;
using MetaFlow.Api.Features.Columns.GetColumns;
using MetaFlow.Api.Features.Columns.ReorderColumns;
using MetaFlow.Api.Features.Columns.UpdateColumn;
using MetaFlow.Contracts.Columns;
using System.Security.Claims;

namespace MetaFlow.Api.Features.Columns
{
    public class ColumnsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/boards/{boardId:guid}/columns")
                .WithTags("Columns")
                .RequireAuthorization();

            // Create Column
            group.MapPost("", async (
                Guid boardId,
                CreateColumnRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new CreateColumnCommand(
                    boardId,
                    request.Name,
                    request.Description,
                    request.ColumnType,
                    request.WipLimit,
                    request.Color,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("CreateColumn")
            .Produces<ColumnResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            // Get Columns
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

                var query = new GetColumnsQuery(boardId, userId);
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetColumns")
            .Produces<List<ColumnListResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            // Update Column
            group.MapPatch("{columnId:guid}", async (
                Guid boardId,
                Guid columnId,
                UpdateColumnRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateColumnCommand(
                    columnId,
                    userId,
                    request.Name,
                    request.Description,
                    request.WipLimit,
                    request.Color,
                    request.IsVisible
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("UpdateColumn")
            .Produces<ColumnResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            // Reorder Columns
            group.MapPut("reorder", async (
                Guid boardId,
                ReorderColumnsRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new ReorderColumnsCommand(
                    boardId,
                    userId,
                    request.Columns
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("ReorderColumns")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            // Delete Column
            group.MapDelete("{columnId:guid}", async (
                Guid boardId,
                Guid columnId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new DeleteColumnCommand(columnId, userId);
                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("DeleteColumn")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}