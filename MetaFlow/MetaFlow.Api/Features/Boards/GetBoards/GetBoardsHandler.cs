using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Boards;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Boards.GetBoards
{
    public class GetBoardsHandler : IRequestHandler<GetBoardsQuery, Result<List<BoardListResponse>>>
    {
        private readonly SupabaseService _supabaseService;

        public GetBoardsHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<List<BoardListResponse>>> Handle(
            GetBoardsQuery request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var boardsQuery = client
                .From<Board>()
                .Select("id,name,description,owner_id,methodology_preset_id,is_public,is_archived,updated_at")
                .Filter("owner_id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString());

            if (!request.IncludeArchived)
            {
                boardsQuery = boardsQuery.Filter("is_archived", Supabase.Postgrest.Constants.Operator.Equals, "false");
            }

            var boardsResponse = await boardsQuery
                .Order("updated_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            var boards = boardsResponse.Models;

            if (boards.Count == 0)
            {
                return Result.Success(new List<BoardListResponse>());
            }

            var methodologyIds = boards.Select(b => b.MethodologyPresetId).Distinct().ToList();

            var methodologies = new Dictionary<Guid, string>();
            foreach (var methodologyId in methodologyIds)
            {
                var methodology = await client
                    .From<MethodologyPreset>()
                    .Select("id,display_name")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, methodologyId.ToString())
                    .Single();

                if (methodology != null)
                {
                    methodologies[methodologyId] = methodology.DisplayName;
                }
            }

            var user = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                .Single();

            var username = user?.Username ?? "Unknown";

            var response = boards.Select(b => new BoardListResponse(
                b.Id,
                b.Name,
                b.Description,
                b.OwnerId,
                username,
                methodologies.GetValueOrDefault(b.MethodologyPresetId, "Unknown"),
                b.IsPublic,
                b.IsArchived,
                b.UpdatedAt
            )).ToList();

            return Result.Success(response);
        }
    }
}