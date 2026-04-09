using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Comments;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Comments.CreateComment
{
    public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<CommentResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public CreateCommentHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<CommentResponse>> Handle(
            CreateCommentCommand request,
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
                return Result.Failure<CommentResponse>("Card not found");
            }

            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<CommentResponse>("Board not found");
            }

            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await client
                    .From<CardComment>()
                    .Select("id,card_id")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.ParentCommentId.Value.ToString())
                    .Single();

                if (parentComment == null || parentComment.CardId != request.CardId)
                {
                    return Result.Failure<CommentResponse>("Parent comment not found or belong to different card");
                }
            }

            var user = await client
                .From<User>()
                .Select("id,username,avatar_url")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                .Single();

            if (user == null)
            {
                return Result.Failure<CommentResponse>("User not found");
            }

            var comment = new CardComment
            {
                Id = Guid.NewGuid(),
                CardId = request.CardId,
                UserId = request.UserId,
                ParentCommentId = request.ParentCommentId,
                Content = request.Content,
                Metadata = new CommentMetadata(),
                IsEdited = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await client
                .From<CardComment>()
                .Insert(comment);

            var createdComment = insertResponse.Models.FirstOrDefault();

            if (createdComment == null)
            {
                return Result.Failure<CommentResponse>("Failed to create comment");
            }

            var response = new CommentResponse(
                createdComment.Id,
                createdComment.CardId,
                createdComment.UserId,
                user.Username,
                user.AvatarUrl,
                createdComment.ParentCommentId,
                createdComment.Content,
                createdComment.IsEdited,
                createdComment.IsDeleted,
                createdComment.CreatedAt,
                createdComment.UpdatedAt,
                new List<CommentResponse>()
                );

            return Result.Success(response);
        }
    }
}