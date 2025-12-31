using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Cards.GetCards
{
    public class GetCardsHandler : IRequestHandler<GetCardsQuery, Result<List<CardListResponse>>>
    {
        private readonly SupabaseService _supabaseService;

        public GetCardsHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<List<CardListResponse>>> Handle(
            GetCardsQuery request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<List<CardListResponse>>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<List<CardListResponse>>("Access denied");
            }

            var cardsQuery = client
                .From<Card>()
                .Select("id,title,priority,status,due_date,assigned_to_id,updated_at,column_id,position,description,is_archived")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString());

            if (request.ColumnId.HasValue)
            {
                cardsQuery = cardsQuery.Filter("column_id", Supabase.Postgrest.Constants.Operator.Equals, request.ColumnId.Value.ToString());
            }

            if (!request.IncludeArchived)
            {
                cardsQuery = cardsQuery.Filter("is_archived", Supabase.Postgrest.Constants.Operator.Equals, "false");
            }

            var cardsResponse = await cardsQuery
                .Order("position", Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            var cards = cardsResponse.Models;

            if (cards.Count == 0)
            {
                return Result.Success(new List<CardListResponse>());
            }

            var assignedUserIds = cards
                .Where(c => c.AssignedToId.HasValue)
                .Select(c => c.AssignedToId!.Value)
                .Distinct()
                .ToList();

            var assignedUsers = new Dictionary<Guid, string>();
            foreach (var userId in assignedUserIds)
            {
                var user = await client
                    .From<User>()
                    .Select("id,username")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Single();

                if (user != null)
                {
                    assignedUsers[userId] = user.Username;
                }
            }

            var cardCounts = new Dictionary<Guid, (int comments, int attachments)>();
            foreach (var card in cards)
            {
                var commentsResponse = await client
                    .From<CardComment>()
                    .Select("id")
                    .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, card.Id.ToString())
                    .Filter("is_deleted", Supabase.Postgrest.Constants.Operator.Equals, "false")
                    .Get();

                var attachmentsResponse = await client
                    .From<CardAttachment>()
                    .Select("id")
                    .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, card.Id.ToString())
                    .Get();

                cardCounts[card.Id] = (commentsResponse.Models.Count, attachmentsResponse.Models.Count);
            }

            var response = cards.Select(c => new CardListResponse(
                c.Id,
                c.Title,
                c.Description,
                c.ColumnId,
                c.Position,
                c.Priority,
                c.Status,
                c.DueDate,
                c.AssignedToId,
                c.AssignedToId.HasValue ? assignedUsers.GetValueOrDefault(c.AssignedToId.Value) : null,
                cardCounts.GetValueOrDefault(c.Id).comments,
                cardCounts.GetValueOrDefault(c.Id).attachments,
                c.UpdatedAt,
                c.IsArchived
            )).ToList();

            return Result.Success(response);
        }
    }
}