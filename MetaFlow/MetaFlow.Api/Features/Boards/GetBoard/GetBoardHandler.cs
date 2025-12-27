using MediatR;
using MetaFlow.Api.Common;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Boards;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Boards.GetBoard
{
    public class GetBoardHandler : IRequestHandler<GetBoardQuery, Result<BoardResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public GetBoardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<BoardResponse>> Handle(
            GetBoardQuery request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,name,description,owner_id,methodology_preset_id,is_public,is_template,is_archived,archived_at,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<BoardResponse>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<BoardResponse>("Access denied");
            }

            var owner = await client
                .From<User>()
                .Select("id,email,username,full_name,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, board.OwnerId.ToString())
                .Single();

            var methodology = await client
                .From<MethodologyPreset>()
                .Select("id,name,display_name,description,icon,category,is_system,is_active,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, board.MethodologyPresetId.ToString())
                .Single();

            var response = new BoardResponse(
                board.Id,
                board.Name,
                board.Description,
                board.OwnerId,
                owner?.Username ?? "Unknown",
                board.MethodologyPresetId,
                methodology?.DisplayName ?? "Unknown",
                board.IsPublic,
                board.IsTemplate,
                board.IsArchived,
                board.CreatedAt,
                board.UpdatedAt
            );

            return Result.Success(response);
        }
    }


}
