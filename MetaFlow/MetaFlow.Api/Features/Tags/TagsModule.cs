using Carter;
using MediatR;
using MetaFlow.Api.Features.Tags.AddTagToCard;
using MetaFlow.Api.Features.Tags.CreateTag;
using MetaFlow.Api.Features.Tags.DeleteTag;
using MetaFlow.Api.Features.Tags.GetCardTags;
using MetaFlow.Api.Features.Tags.GetTags;
using MetaFlow.Api.Features.Tags.RemoveTagFromCard;
using MetaFlow.Api.Features.Tags.UpdateTag;
using MetaFlow.Contracts.Tags;
using System.Security.Claims;

namespace MetaFlow.Api.Features.Tags
{
    public class TagsModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var boardTagsGroup = app.MapGroup("api/boards/{boardId:guid}/tags")
                .WithTags("Tags")
                .RequireAuthorization();

            var cardTagsGroup = app.MapGroup("api/cards/{cardId:guid}/tags")
                .WithTags("Tags")
                .RequireAuthorization();

            boardTagsGroup.MapPost("", async (
                Guid boardId,
                CreateTagRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new CreateTagCommand(
                    boardId,
                    request.Name,
                    request.Color,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("CreateTag")
            .Produces<TagResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            boardTagsGroup.MapGet("", async (
                Guid boardId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetTagsQuery(boardId, userId);
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetBoardTags")
            .Produces<List<TagResponse>>(StatusCodes.Status200OK);

            boardTagsGroup.MapPatch("{tagId:guid}", async (
                Guid boardId,
                Guid tagId,
                UpdateTagRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new UpdateTagCommand(
                    tagId,
                    userId,
                    request.Name,
                    request.Color
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("UpdateTag")
            .Produces<TagResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            boardTagsGroup.MapDelete("{tagId:guid}", async (
                Guid boardId,
                Guid tagId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new DeleteTagCommand(tagId, userId);
                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("DeleteTag")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

            cardTagsGroup.MapPost("", async (
                Guid cardId,
                AddTagToCardRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new AddTagToCardCommand(
                    cardId,
                    request.TagId,
                    userId
                );

                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("AddTagToCard")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

            cardTagsGroup.MapGet("", async (
                Guid cardId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var query = new GetCardTagsQuery(cardId, userId);
                var result = await sender.Send(query);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("GetCardTags")
            .Produces<List<TagResponse>>(StatusCodes.Status200OK);

            cardTagsGroup.MapDelete("{tagId:guid}", async (
                Guid cardId,
                Guid tagId,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userIdClaim = user.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Results.Unauthorized();
                }

                var command = new RemoveTagFromCardCommand(cardId, tagId, userId);
                var result = await sender.Send(command);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.BadRequest(new { error = result.Error });
            })
            .WithName("RemoveTagFromCard")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
