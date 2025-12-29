using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Columns.ReorderColumns
{
    public class ReorderColumnsHandler : IRequestHandler<ReorderColumnsCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public ReorderColumnsHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            ReorderColumnsCommand request,
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
                return Result.Failure<bool>("Board not found");
            }

            if (board.OwnerId != request.UserId)
            {
                return Result.Failure<bool>("Only board owner can reorder columns");
            }

            // Update positions for each column
            foreach (var columnPosition in request.Columns)
            {
                var column = await client
                    .From<Column>()
                    .Select("id,board_id,name,description,column_type,wip_limit,color,is_visible,created_at,position")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, columnPosition.ColumnId.ToString())
                    .Single();

                if (column != null && column.BoardId == request.BoardId)
                {
                    column.Position = columnPosition.Position;
                    column.UpdatedAt = DateTime.UtcNow;

                    await client
                        .From<Column>()
                        .Update(column);
                }
            }

            return Result.Success(true);
        }
    }
}
