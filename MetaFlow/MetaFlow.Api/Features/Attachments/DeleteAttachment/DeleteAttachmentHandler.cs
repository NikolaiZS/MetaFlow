using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Attachments.DeleteAttachment
{
    public class DeleteAttachmentHandler : IRequestHandler<DeleteAttachmentCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public DeleteAttachmentHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            DeleteAttachmentCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var attachment = await client
                .From<CardAttachment>()
                .Select("id,card_id,uploaded_by_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.AttachmentId.ToString())
                .Single();

            if (attachment == null)
            {
                return Result.Failure<bool>("Attachment not found");
            }

            var card = await client
                .From<Card>()
                .Select("id,board_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, attachment.CardId.ToString())
                .Single();

            if (card == null)
            {
                return Result.Failure<bool>("Card not found");
            }

            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (attachment.UploadedById != request.UserId && board?.OwnerId != request.UserId)
            {
                return Result.Failure<bool>("Only attachment uploader or board owner can delete the attachment");
            }

            await client
                .From<CardAttachment>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.AttachmentId.ToString())
                .Delete();

            // TODO: Delete file from Supabase Storage
            // var storage = client.Storage.From("attachments");
            // await storage.Remove(new[] { attachment.FileUrl });

            return Result.Success(true);
        }
    }
}
