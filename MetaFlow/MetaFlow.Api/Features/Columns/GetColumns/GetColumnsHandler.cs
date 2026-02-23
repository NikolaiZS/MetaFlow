using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Columns;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;
using MetaFlow.Infrastructure.Services.Cache;

namespace MetaFlow.Api.Features.Columns.GetColumns
{
    public class GetColumnsHandler : IRequestHandler<GetColumnsQuery, Result<List<ColumnListResponse>>>
    {
        private readonly SupabaseService _supabaseService;
        private readonly ICacheService _cache;

        public GetColumnsHandler(SupabaseService supabaseService, ICacheService cache)
        {
            _supabaseService = supabaseService;
            _cache = cache;
        }

        public async Task<Result<List<ColumnListResponse>>> Handle(
            GetColumnsQuery request,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"columns:{request.BoardId}:user:{request.UserId}";
            var cached = await _cache.GetAsync<List<ColumnListResponse>>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                return Result.Success(cached);
            }

            var client = _supabaseService.GetClient();

            // Verify board exists and user has access
            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<List<ColumnListResponse>>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<List<ColumnListResponse>>("Access denied");
            }

            // Get columns
            var columnsResponse = await client
                .From<Column>()
                .Select("id,name,position,column_type,wip_limit,color,is_visible,board_id")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Filter("is_visible", Supabase.Postgrest.Constants.Operator.Equals, "true")
                .Order("position", Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            var columns = columnsResponse.Models;

            // Get card counts for each column
            var cardCounts = new Dictionary<Guid, int>();

            foreach (var column in columns)
            {
                var cardsResponse = await client
                    .From<Card>()
                    .Select("id")
                    .Filter("column_id", Supabase.Postgrest.Constants.Operator.Equals, column.Id.ToString())
                    .Filter("is_archived", Supabase.Postgrest.Constants.Operator.Equals, "false")
                    .Get();

                cardCounts[column.Id] = cardsResponse.Models.Count;
            }

            var response = columns.Select(c => new ColumnListResponse(
                c.Id,
                c.Name,
                c.Position,
                c.ColumnType,
                c.WipLimit,
                c.Color,
                c.IsVisible,
                cardCounts.GetValueOrDefault(c.Id, 0)
            )).ToList();

            await _cache.SetAsync(cacheKey, response, TimeSpan.FromSeconds(30), cancellationToken);

            return Result.Success(response);
        }
    }
}