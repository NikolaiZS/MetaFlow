using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Cards.MoveCard
{
    public class MoveCardHandler : IRequestHandler<MoveCardCommand, Result<CardResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public MoveCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<CardResponse>> Handle(
            MoveCardCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            // Get card
            var card = await client
                .From<Card>()
                .Select("id,board_id,column_id,title,description,position,priority,status,due_date,start_date,completed_at,created_by_id,assigned_to_id,sprint_id,is_archived,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Single();

            if (card == null)
            {
                return Result.Failure<CardResponse>("Card not found");
            }

            // Verify board access
            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<CardResponse>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<CardResponse>("Access denied");
            }

            // Verify target column exists and belongs to same board
            var targetColumn = await client
                .From<Column>()
                .Select("id,board_id,name,wip_limit")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.TargetColumnId.ToString())
                .Single();

            if (targetColumn == null || targetColumn.BoardId != card.BoardId)
            {
                return Result.Failure<CardResponse>("Target column not found or does not belong to this board");
            }

            // Check WIP limit if moving to different column
            if (card.ColumnId != request.TargetColumnId && targetColumn.WipLimit.HasValue)
            {
                var cardsInTargetColumn = await client
                    .From<Card>()
                    .Select("id")
                    .Filter("column_id", Supabase.Postgrest.Constants.Operator.Equals, request.TargetColumnId.ToString())
                    .Filter("is_archived", Supabase.Postgrest.Constants.Operator.Equals, "false")
                    .Get();

                if (cardsInTargetColumn.Models.Count >= targetColumn.WipLimit.Value)
                {
                    return Result.Failure<CardResponse>($"Target column has reached its WIP limit of {targetColumn.WipLimit.Value}");
                }
            }

            var oldColumnId = card.ColumnId;

            // Update card
            card.ColumnId = request.TargetColumnId;
            card.Position = request.Position;
            card.UpdatedAt = DateTime.UtcNow;

            await client
                .From<Card>()
                .Update(card);

            // Create history entry
            var history = new CardHistory
            {
                Id = Guid.NewGuid(),
                CardId = card.Id,
                UserId = request.UserId,
                Action = "moved",
                FromColumnId = oldColumnId,
                ToColumnId = request.TargetColumnId,
                Changes = new Domain.Models.HistoryChanges
                {
                    Before = new Dictionary<string, object> { { "column_id", oldColumnId } },
                    After = new Dictionary<string, object> { { "column_id", request.TargetColumnId } },
                    ModifiedFields = new List<string> { "column_id" }
                },
                CreatedAt = DateTime.UtcNow
            };

            await client
                .From<CardHistory>()
                .Insert(history);

            // Get updated info
            var creator = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.CreatedById.ToString())
                .Single();

            User? assignedUser = null;
            if (card.AssignedToId.HasValue)
            {
                assignedUser = await client
                    .From<User>()
                    .Select("id,username")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.AssignedToId.Value.ToString())
                    .Single();
            }

            var response = new CardResponse(
                card.Id,
                card.BoardId,
                card.ColumnId,
                targetColumn.Name,
                card.Title,
                card.Description,
                card.Position,
                card.Priority,
                card.Status,
                card.DueDate,
                card.StartDate,
                card.CompletedAt,
                card.CreatedById,
                creator?.Username ?? "Unknown",
                card.AssignedToId,
                assignedUser?.Username,
                card.IsArchived,
                card.CreatedAt,
                card.UpdatedAt
            );

            return Result.Success(response);
        }
    }
}