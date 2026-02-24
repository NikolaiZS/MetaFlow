using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Comments;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Comments.UpdateComment
{
    public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, Result<CommentResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public UpdateCommentHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<CommentResponse>> Handle(
            UpdateCommentCommand request,
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
                return Result.Failure<CommentResponse>("Comment not found");
            }

            if (comment.UserId != request.UserId)
            {
                return Result.Failure<CommentResponse>("Only comment author can edit the comment");
            }

            if (comment.IsDeleted)
            {
                return Result.Failure<CommentResponse>("Cannot edit deleted comment");
            }

            comment.Content = request.Content;
            comment.IsEdited = true;
            comment.UpdatedAt = DateTime.UtcNow;

            await client
                .From<CardComment>()
                .Update(comment);

            var user = await client
                .From<User>()
                .Select("id,username,avatar_url")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, comment.UserId.ToString())
                .Single();

            var response = new CommentResponse(
                comment.Id,
                comment.CardId,
                comment.UserId,
                user?.Username ?? "Unknown",
                user?.AvatarUrl,
                comment.ParentCommentId,
                comment.Content,
                comment.IsEdited,
                comment.IsDeleted,
                comment.CreatedAt,
                comment.UpdatedAt,
                new List<CommentResponse>()
            );

            return Result.Success(response);
        }
    }
}
