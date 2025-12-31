using Carter;
using MediatR;
using MetaFlow.Api.Features.Attachments.DeleteAttachment;
using MetaFlow.Api.Features.Attachments.GetAttachments;
using MetaFlow.Api.Features.Attachments.UploadAttachment;
using MetaFlow.Contracts.Attachments;
using System.Security.Claims;

namespace MetaFlow.Api.Features.Attachments;

public class AttachmentsModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/cards/{cardId:guid}/attachments")
            .WithTags("Attachments")
            .RequireAuthorization();

        // Upload Attachment
        group.MapPost("", async (
            Guid cardId,
            UploadAttachmentRequest request,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new UploadAttachmentCommand(
                cardId,
                request.FileName,
                request.FileUrl,
                request.FileSize,
                request.MimeType,
                request.ThumbnailUrl,
                userId
            );

            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("UploadAttachment")
        .Produces<AttachmentResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Get Attachments
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

            var query = new GetAttachmentsQuery(cardId, userId);
            var result = await sender.Send(query);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("GetAttachments")
        .Produces<List<AttachmentResponse>>(StatusCodes.Status200OK);

        // Delete Attachment
        group.MapDelete("{attachmentId:guid}", async (
            Guid cardId,
            Guid attachmentId,
            ClaimsPrincipal user,
            ISender sender) =>
        {
            var userIdClaim = user.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var command = new DeleteAttachmentCommand(attachmentId, userId);
            var result = await sender.Send(command);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("DeleteAttachment")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest);
    }
}
