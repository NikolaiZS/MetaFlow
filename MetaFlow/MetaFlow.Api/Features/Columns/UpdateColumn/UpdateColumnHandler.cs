using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Columns;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Columns.UpdateColumn
{
    public class UpdateColumnHandler : IRequestHandler<UpdateColumnCommand, Result<ColumnResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public UpdateColumnHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<ColumnResponse>> Handle(
            UpdateColumnCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            // Get column
            var column = await client
                .From<Column>()
                .Select("id,board_id,name,description,position,column_type,wip_limit,color,is_visible,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.ColumnId.ToString())
                .Single();

            if (column == null)
            {
                return Result.Failure<ColumnResponse>("Column not found");
            }

            // Verify board ownership
            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, column.BoardId.ToString())
                .Single();

            if (board == null || board.OwnerId != request.UserId)
            {
                return Result.Failure<ColumnResponse>("Only board owner can update columns");
            }

            // Update fields
            if (request.Name != null) column.Name = request.Name;
            if (request.Description != null) column.Description = request.Description;
            if (request.WipLimit.HasValue) column.WipLimit = request.WipLimit.Value;
            if (request.Color != null) column.Color = request.Color;
            if (request.IsVisible.HasValue) column.IsVisible = request.IsVisible.Value;

            column.UpdatedAt = DateTime.UtcNow;

            // Update in database
            await client
                .From<Column>()
                .Update(column);

            // Get card count
            var cardsResponse = await client
                .From<Card>()
                .Select("id")
                .Filter("column_id", Supabase.Postgrest.Constants.Operator.Equals, column.Id.ToString())
                .Filter("is_archived", Supabase.Postgrest.Constants.Operator.Equals, "false")
                .Get();

            var response = new ColumnResponse(
                column.Id,
                column.BoardId,
                column.Name,
                column.Description,
                column.Position,
                column.ColumnType,
                column.WipLimit,
                column.Color,
                column.IsVisible,
                cardsResponse.Models.Count,
                column.CreatedAt,
                column.UpdatedAt
            );

            return Result.Success(response);
        }
    }
}