using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Tags;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Tags.GetTags
{
    public class GetTagsHandler : IRequestHandler<GetTagsQuery, Result<List<TagResponse>>>
    {
        private readonly SupabaseService _supabaseService;

        public GetTagsHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
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

            return Result.Success(response);
        }
    }
}
