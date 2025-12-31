using Carter;
using MediatR;
using MetaFlow.Api.Features.Cards.ArchiveCard;
using MetaFlow.Api.Features.Cards.CreateCard;
using MetaFlow.Api.Features.Cards.DeleteCard;
using MetaFlow.Api.Features.Cards.GetCard;
using MetaFlow.Api.Features.Cards.GetCards;
using MetaFlow.Api.Features.Cards.MoveCard;
using MetaFlow.Api.Features.Cards.UpdateCard;
using MetaFlow.Contracts.Cards;
using System.Security.Claims;

namespace MetaFlow.Api.Features.Cards;

public class CardsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/boards/{boardId:guid}/cards")
            .WithTags("Cards")
            .RequireAuthorization();

        group.MapPost("", async (
            Guid boardId,
            CreateCardRequest request,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new CreateCardCommand(
                boardId,
                request.Title,
                request.Description,
                request.ColumnId,
                request.Priority,
                request.DueDate,
                request.StartDate,
                request.AssignedToId,
                userId
            );

            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("CreateCard")
        .Produces<CardResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("", async (
            Guid boardId,
            Guid? columnId,
            bool includeArchived,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetCardsQuery(boardId, columnId, userId, includeArchived);
            var result = await sender.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("GetCards")
        .Produces<List<CardListResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("{cardId:guid}", async (
            Guid boardId,
            Guid cardId,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var query = new GetCardQuery(cardId, userId);
            var result = await sender.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("GetCard")
        .Produces<CardResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPatch("{cardId:guid}", async (
            Guid boardId,
            Guid cardId,
            UpdateCardRequest request,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new UpdateCardCommand(
                cardId,
                userId,
                request.Title,
                request.Description,
                request.Priority,
                request.Status,
                request.DueDate,
                request.StartTime,
                request.AssignedToId
            );

            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("UpdateCard")
        .Produces<CardResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("{cardId:guid}/move", async (
            Guid boardId,
            Guid cardId,
            MoveCardRequest request,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new MoveCardCommand(
                cardId,
                request.TargetColumnId,
                request.Position,
                userId
            );

            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("MoveCard")
        .Produces<CardResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("{cardId:guid}/archive", async (
            Guid boardId,
            Guid cardId,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new ArchiveCardCommand(cardId, userId, true);
            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("ArchiveCard")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapPut("{cardId:guid}/unarchive", async (
            Guid boardId,
            Guid cardId,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new ArchiveCardCommand(cardId, userId, false);
            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("UnarchiveCard")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);

        group.MapDelete("{cardId:guid}", async (
            Guid boardId,
            Guid cardId,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new DeleteCardCommand(cardId, userId);
            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("DeleteCard")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized);
    }
}