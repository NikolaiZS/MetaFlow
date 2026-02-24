using Carter;
using MediatR;
using MetaFlow.Api.Features.Comments.CreateComment;
using MetaFlow.Api.Features.Comments.DeleteComment;
using MetaFlow.Api.Features.Comments.GetComments;
using MetaFlow.Api.Features.Comments.UpdateComment;
using MetaFlow.Contracts.Comments;
using System.Security.Claims;

namespace MetaFlow.Api.Features.Comments
{
    public class CommentsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/cards/{cardId:guid}/comments")
                .WithTags("Comments")
                .RequireAuthorization();

            group.MapPost("", async (
                Guid cardId,
                CreateCommentRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new CreateCommentCommand(
                    cardId,
                    request.Content,
                    request.ParentCommentId,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("CreateComment")
            .Produces<CommentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("", async (
                Guid cardId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetCommentsQuery(cardId, userId);
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetComments")
            .Produces<List<CommentResponse>>(StatusCodes.Status200OK);

            group.MapPatch("{commentId:guid}", async (
                Guid cardId,
                Guid commentId,
                UpdateCommentRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateCommentCommand(
                    commentId,
                    request.Content,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("UpdateComment")
            .Produces<CommentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            group.MapDelete("{commentId:guid}", async (
                Guid cardId,
                Guid commentId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new DeleteCommentCommand(commentId, userId);
                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("DeleteComment")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
