using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Cards.GetCard
{
    public class GetCardHandler : IRequestHandler<GetCardQuery, Result<CardResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public GetCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<CardResponse>> Handle(
            GetCardQuery request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var card = await client
                .From<Card>()
                .Select("id,board_id,column_id,title,description,position,priority,status,due_date,start_date,completed_at,created_by_id,assigned_to_id,is_archived,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Single();

            if (card == null)
            {
                return Result.Failure<CardResponse>("Card not found");
            }

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
