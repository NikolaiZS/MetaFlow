using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Attachments;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Attachments.GetAttachments;

public class GetAttachmentsHandler : IRequestHandler<GetAttachmentsQuery, Result<List<AttachmentResponse>>>
{
    private readonly SupabaseService _supabaseService;
    private readonly ICacheService _cache;

    public GetAttachmentsHandler(SupabaseService supabaseService, ICacheService cache)
    {
        _supabaseService = supabaseService;
        _cache = cache;
    }

    public async Task<Result<List<AttachmentResponse>>> Handle(
        GetAttachmentsQuery request,
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
            return Result.Failure<List<AttachmentResponse>>("Card not found");
        }

        var board = await client
            .From<Board>()
            .Select("id,owner_id,is_public")
            .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
            .Single();

        if (board == null)
        {
            return Result.Failure<List<AttachmentResponse>>("Board not found");
        }

        if (board.OwnerId != request.UserId && !board.IsPublic)
        {
            return Result.Failure<List<AttachmentResponse>>("Access denied");
        }

        var cacheKey = $"attachments:{request.CardId}";
        var cached = await _cache.GetAsync<List<AttachmentResponse>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result.Success(cached);
        }

        var attachmentsResponse = await client
            .From<CardAttachment>()
            .Select("id,card_id,uploaded_by_id,file_name,file_url,file_size,mime_type,thumbnail_url,uploaded_at")
            .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
            .Order("uploaded_at", Supabase.Postgrest.Constants.Ordering.Descending)
            .Get();

        var attachments = attachmentsResponse.Models;

        if (attachments.Count == 0)
        {
            var empty = new List<AttachmentResponse>();
            await _cache.SetAsync(cacheKey, empty, TimeSpan.FromMinutes(5), cancellationToken);
            return Result.Success(empty);
        }

        var uploaderIds = attachments.Select(a => a.UploadedById).Distinct().ToList();

        var uploaders = new Dictionary<Guid, string>();
        foreach (var uploaderId in uploaderIds)
        {
            var user = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, uploaderId.ToString())
                .Single();

            if (user != null)
            {
                uploaders[uploaderId] = user.Username;
            }
        }

        var response = attachments.Select(a => new AttachmentResponse(
            a.Id,
            a.CardId,
            a.UploadedById,
            uploaders.GetValueOrDefault(a.UploadedById, "Unknown"),
            a.FileName,
            a.FileUrl,
            a.FileSize,
            a.MimeType,
            a.ThumbnailUrl,
            a.UploadedAt
        )).ToList();

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);

        return Result.Success(response);
    }
}
