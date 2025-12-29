using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Boards;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Boards.UpdateBoard
{
    public class UpdateBoardHandler : IRequestHandler<UpdateBoardCommand, Result<BoardResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public UpdateBoardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<BoardResponse>> Handle(
            UpdateBoardCommand request,
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

            if (board.OwnerId != request.UserId)
            {
                return Result.Failure<BoardResponse>("Only board owner can update");
            }

            if (request.Name != null) board.Name = request.Name;
            if (request.Description != null) board.Description = request.Description;
            if (request.IsPublic.HasValue) board.IsPublic = request.IsPublic.Value;
            if (request.IsArchived.HasValue)
            {
                board.IsArchived = request.IsArchived.Value;
                board.ArchivedAt = request.IsArchived.Value ? DateTime.UtcNow : null;
            }

            board.UpdatedAt = DateTime.UtcNow;

            await client
                .From<Board>()
                .Update(board);

            var owner = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, board.OwnerId.ToString())
                .Single();

            var methodology = await client
                .From<MethodologyPreset>()
                .Select("id,display_name")
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
