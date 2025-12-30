using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Columns.DeleteColumn
{
    public class DeleteColumnHandler : IRequestHandler<DeleteColumnCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public DeleteColumnHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            DeleteColumnCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            // Get column
            var column = await client
                .From<Column>()
                .Select("id,board_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.ColumnId.ToString())
                .Single();

            if (column == null)
            {
                return Result.Failure<bool>("Column not found");
            }

            // Verify board ownership
            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, column.BoardId.ToString())
                .Single();

            if (board == null || board.OwnerId != request.UserId)
            {
                return Result.Failure<bool>("Only board owner can delete columns");
            }

            // Check if column has cards
            var cardsResponse = await client
                .From<Card>()
                .Select("id")
                .Filter("column_id", Supabase.Postgrest.Constants.Operator.Equals, request.ColumnId.ToString())
                .Filter("is_archived", Supabase.Postgrest.Constants.Operator.Equals, "false")
                .Get();

            if (cardsResponse.Models.Count > 0)
            {
                return Result.Failure<bool>("Cannot delete column with active cards. Move or archive cards first.");
            }

            // Delete column
            await client
                .From<Column>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.ColumnId.ToString())
                .Delete();

            return Result.Success(true);
        }
    }
}