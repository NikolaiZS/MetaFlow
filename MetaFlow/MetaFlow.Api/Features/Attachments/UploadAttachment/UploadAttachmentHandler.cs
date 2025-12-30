using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Attachments;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Attachments.UploadAttachment
{
    public class UploadAttachmentHandler : IRequestHandler<UploadAttachmentCommand, Result<AttachmentResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public UploadAttachmentHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<AttachmentResponse>> Handle(
            UploadAttachmentCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var card = await client
                .From<Card>()
                .Select("id,board_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Single();

            if (card == null)
            {
                return Result.Failure<AttachmentResponse>("Card not found");
            }

            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<AttachmentResponse>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<AttachmentResponse>("Access denied");
            }

            var user = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                .Single();

            if (user == null)
            {
                return Result.Failure<AttachmentResponse>("User not found");
            }

            var attachment = new CardAttachment
            {
                Id = Guid.NewGuid(),
                CardId = request.CardId,
                UploadedById = request.UserId,
                FileName = request.FileName,
                FileUrl = request.FileUrl,
                FileSize = request.FileSize,
                MimeType = request.MimeType,
                ThumbnailUrl = request.ThumbnailUrl,
                UploadedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await client
                .From<CardAttachment>()
                .Insert(attachment);

            var createdAttachment = insertResponse.Models.FirstOrDefault();

            if (createdAttachment == null)
            {
                return Result.Failure<AttachmentResponse>("Failed to create attachment");
            }

            var response = new AttachmentResponse(
                createdAttachment.Id,
                createdAttachment.CardId,
                createdAttachment.UploadedById,
                user.Username,
                createdAttachment.FileName,
                createdAttachment.FileUrl,
                createdAttachment.FileSize,
                createdAttachment.MimeType,
                createdAttachment.ThumbnailUrl,
                createdAttachment.UploadedAt
            );

            return Result.Success(response);
        }
    }
}
