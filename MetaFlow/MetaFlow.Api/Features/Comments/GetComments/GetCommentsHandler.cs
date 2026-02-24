using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Comments;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Comments.GetComments;

public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, Result<List<CommentResponse>>>
{
    private readonly SupabaseService _supabaseService;
    private readonly ICacheService _cache;

    public GetCommentsHandler(SupabaseService supabaseService, ICacheService cache)
    {
        _supabaseService = supabaseService;
        _cache = cache;
    }

    public async Task<Result<List<CommentResponse>>> Handle(
        GetCommentsQuery request,
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
            return Result.Failure<List<CommentResponse>>("Card not found");
        }

        var board = await client
            .From<Board>()
            .Select("id,owner_id,is_public")
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
            .Single();

        if (board == null)
        {
            return Result.Failure<List<CommentResponse>>("Board not found");
        }

        if (board.OwnerId != request.UserId && !board.IsPublic)
        {
            return Result.Failure<List<CommentResponse>>("Access denied");
        }

        var cacheKey = $"comments:{request.CardId}";
        var cached = await _cache.GetAsync<List<CommentResponse>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var commentsResponse = await client
            .From<CardComment>()
            .Select("id,card_id,user_id,parent_comment_id,content,is_edited,is_deleted,deleted_at,created_at,updated_at")
            .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
            .Order("created_at", Supabase.Postgrest.Constants.Ordering.Ascending)
            .Get();

        var comments = commentsResponse.Models;

        if (comments.Count == 0)
        {
            var empty = new List<CommentResponse>();
            await _cache.SetAsync(cacheKey, empty, TimeSpan.FromMinutes(5), cancellationToken);
            return Result.Success(empty);
        }

        var userIds = comments.Select(c => c.UserId).Distinct().ToList();

        var users = new Dictionary<Guid, (string username, string? avatarUrl)>();
        foreach (var userId in userIds)
        {
            var user = await client
                .From<User>()
                .Select("id,username,avatar_url")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                .Single();

            if (user != null)
            {
                users[userId] = (user.Username, user.AvatarUrl);
            }
        }

        var commentMap = new Dictionary<Guid, CommentResponse>();
        var topLevelComments = new List<CommentResponse>();

        foreach (var comment in comments)
        {
            var userInfo = users.GetValueOrDefault(comment.UserId, ("Unknown", null));

            var commentResponse = new CommentResponse(
                comment.Id,
                comment.CardId,
                comment.UserId,
                userInfo.username,
                userInfo.avatarUrl,
                comment.ParentCommentId,
                comment.IsDeleted ? "[Deleted]" : comment.Content,
                comment.IsEdited,
                comment.IsDeleted,
                comment.CreatedAt,
                comment.UpdatedAt,
                new List<CommentResponse>()
            );

            commentMap[comment.Id] = commentResponse;
        }

        foreach (var comment in comments)
        {
            var commentResponse = commentMap[comment.Id];

            if (comment.ParentCommentId.HasValue && commentMap.ContainsKey(comment.ParentCommentId.Value))
            {
                var parent = commentMap[comment.ParentCommentId.Value];
                parent.Replies.Add(commentResponse);
            }
            else
            {
                topLevelComments.Add(commentResponse);
            }
        }

        await _cache.SetAsync(cacheKey, topLevelComments, TimeSpan.FromMinutes(5), cancellationToken);

        return Result.Success(topLevelComments);
    }
}
