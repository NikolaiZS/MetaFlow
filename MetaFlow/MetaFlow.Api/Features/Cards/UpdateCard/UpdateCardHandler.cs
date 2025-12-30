using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Cards.UpdateCard
{
    public class UpdateCardHandler : IRequestHandler<UpdateCardCommand, Result<CardResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public UpdateCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<CardResponse>> Handle(
            UpdateCardCommand request,
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
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null || board.OwnerId != request.UserId)
            {
                return Result.Failure<CardResponse>("Only board owner can update cards");
            }

            // Verify assigned user exists (if provided)
            User? assignedUser = null;
            if (request.AssignedToId.HasValue)
            {
                assignedUser = await client
                    .From<User>()
                    .Select("id,username")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.AssignedToId.Value.ToString())
                    .Single();

                if (assignedUser == null)
                {
                    return Result.Failure<CardResponse>("Assigned user not found");
                }
            }

            // Update fields
            if (request.Title != null) card.Title = request.Title;
            if (request.Description != null) card.Description = request.Description;
            if (request.Priority != null) card.Priority = request.Priority;
            if (request.Status != null)
            {
                card.Status = request.Status;
                if (request.Status == "completed" && card.CompletedAt == null)
                {
                    card.CompletedAt = DateTime.UtcNow;
                }
                else if (request.Status != "completed")
                {
                    card.CompletedAt = null;
                }
            }
            if (request.DueDate.HasValue) card.DueDate = request.DueDate;
            if (request.StartDate.HasValue) card.StartDate = request.StartDate;
            if (request.AssignedToId.HasValue) card.AssignedToId = request.AssignedToId;

            card.UpdatedAt = DateTime.UtcNow;

            // Update in database
            await client
                .From<Card>()
                .Update(card);

            // Get column and creator info
            var column = await client
                .From<Column>()
                .Select("id,name")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.ColumnId.ToString())
                .Single();

            var creator = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.CreatedById.ToString())
                .Single();

            var response = new CardResponse(
                card.Id,
                card.BoardId,
                card.ColumnId,
                column?.Name ?? "Unknown",
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
