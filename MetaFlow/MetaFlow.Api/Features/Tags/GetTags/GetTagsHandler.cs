using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Tags;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Tags.GetTags
{
    public class GetTagsHandler : IRequestHandler<GetTagsQuery, Result<List<TagResponse>>>
    {
        private readonly SupabaseService _supabaseService;
        private readonly ICacheService _cache;

        public GetTagsHandler(SupabaseService supabaseService, ICacheService cache)
        {
            _supabaseService = supabaseService;
            _cache = cache;
        }

        public async Task<Result<List<TagResponse>>> Handle(
            GetTagsQuery request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<List<TagResponse>>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<List<TagResponse>>("Access denied");
            }

            var cacheKey = $"tags:{request.BoardId}";
            var cached = await _cache.GetAsync<List<TagResponse>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return Result.Success(cached);
            }

            var tagsResponse = await client
                .From<Tag>()
                .Select("id,board_id,name,color,created_at")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Order("name", Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            var tags = tagsResponse.Models;

            var response = tags.Select(t => new TagResponse(
                t.Id,
                t.BoardId,
                t.Name,
                t.Color,
                t.CreatedAt
            )).ToList();

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);

            return Result.Success(response);
        }
    }
}
