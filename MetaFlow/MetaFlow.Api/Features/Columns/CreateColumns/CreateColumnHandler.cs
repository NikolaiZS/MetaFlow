using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Columns;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Columns.CreateColumns
{
    public class CreateColumnHandler : IRequestHandler<CreateColumnCommand, Result<ColumnResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public CreateColumnHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<ColumnResponse>> Handle(
            CreateColumnCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            // Verify board exists and user has access
            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<ColumnResponse>("Board not found");
            }

            if (board.OwnerId != request.UserId)
            {
                return Result.Failure<ColumnResponse>("Only board owner can create columns");
            }

            // Get current max position
            var existingColumns = await client
                .From<Column>()
                .Select("position")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Order("position", Supabase.Postgrest.Constants.Ordering.Descending)
                .Limit(1)
                .Get();

            var maxPosition = existingColumns.Models.FirstOrDefault()?.Position ?? -1;

            // Create column
            var column = new Column
            {
                Id = Guid.NewGuid(),
                BoardId = request.BoardId,
                Name = request.Name,
                Description = request.Description,
                Position = maxPosition + 1,
                ColumnType = request.ColumnType,
                WipLimit = request.WipLimit,
                Color = request.Color,
                Settings = new ColumnSettings(),
                IsVisible = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await client
                .From<Column>()
                .Insert(column);

            var createdColumn = insertResponse.Models.FirstOrDefault();

            if (createdColumn == null)
            {
                return Result.Failure<ColumnResponse>("Failed to create column");
            }

            var response = new ColumnResponse(
                createdColumn.Id,
                createdColumn.BoardId,
                createdColumn.Name,
                createdColumn.Description,
                createdColumn.Position,
                createdColumn.ColumnType,
                createdColumn.WipLimit,
                createdColumn.Color,
                createdColumn.IsVisible,
                0, // No cards yet
                createdColumn.CreatedAt,
                createdColumn.UpdatedAt
            );

            return Result.Success(response);
        }
    }
}
