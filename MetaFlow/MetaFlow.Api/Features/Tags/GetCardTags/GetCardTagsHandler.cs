using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Tags;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Tags.GetCardTags
{
    public class GetCardTagsHandler : IRequestHandler<GetCardTagsQuery, Result<List<TagResponse>>>
    {
        private readonly SupabaseService _supabaseService;
        private readonly ICacheService _cache;

        public GetCardTagsHandler(SupabaseService supabaseService, ICacheService cache)
        {
            _supabaseService = supabaseService;
            _cache = cache;
        }

        public async Task<Result<List<TagResponse>>> Handle(
            GetCardTagsQuery request,
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
                return Result.Failure<List<TagResponse>>("Card not found");
            }

            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<List<TagResponse>>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<List<TagResponse>>("Access denied");
            }

            var cacheKey = $"card-tags:{request.CardId}";
            var cached = await _cache.GetAsync<List<TagResponse>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return Result.Success(cached);
            }

            var cardTagsResponse = await client
                .From<CardTag>()
                .Select("id,tag_id")
                .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Get();

            var cardTags = cardTagsResponse.Models;

            if (cardTags.Count == 0)
            {
                var empty = new List<TagResponse>();
                await _cache.SetAsync(cacheKey, empty, TimeSpan.FromMinutes(5), cancellationToken);
                return Result.Success(empty);
            }

            var tagIds = cardTags.Select(ct => ct.TagId).ToList();

            var tags = new List<TagResponse>();
            foreach (var tagId in tagIds)
            {
                var tag = await client
                    .From<Tag>()
                    .Select("id,board_id,name,color,created_at")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, tagId.ToString())
                    .Single();

                if (tag != null)
                {
                    tags.Add(new TagResponse(
                        tag.Id,
                        tag.BoardId,
                        tag.Name,
                        tag.Color,
                        tag.CreatedAt
                    ));
                }
            }

            await _cache.SetAsync(cacheKey, tags, TimeSpan.FromMinutes(5), cancellationToken);

            return Result.Success(tags);
        }
    }
}
