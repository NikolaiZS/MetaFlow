using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Comments.DeleteComment
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public DeleteCommentHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            DeleteCommentCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var comment = await client
                .From<CardComment>()
                .Select("id,card_id,user_id,parent_comment_id,content,is_edited,is_deleted,deleted_at,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.CommentId.ToString())
                .Single();

            if (comment == null)
            {
                return Result.Failure<bool>("Comment not found");
            }

            var card = await client
                .From<Card>()
                .Select("id,board_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, comment.CardId.ToString())
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

            if (comment.UserId != request.UserId && board?.OwnerId != request.UserId)
            {
                return Result.Failure<bool>("Only comment author or board owner can delete the comment");
            }

            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            await client
                .From<CardComment>()
                .Update(comment);

            return Result.Success(true);
        }
    }
}
