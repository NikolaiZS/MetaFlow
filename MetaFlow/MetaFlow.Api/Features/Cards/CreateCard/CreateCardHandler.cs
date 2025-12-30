using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Cards;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Cards.CreateCard
{
    public class CreateCardHandler : IRequestHandler<CreateCardCommand, Result<CardResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public CreateCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<CardResponse>> Handle(
            CreateCardCommand request,
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
                return Result.Failure<CardResponse>("Board not found");
            }

            if( board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<CardResponse>("Access denied");
            }

            var column = await client
                .From<Column>()
                .Select("id,board_id,name")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.ColumnId.ToString())
                .Single();

            if (column == null || column.BoardId != request.BoardId)
            {
                return Result.Failure<CardResponse>("Column not found or does not belong to this board");
            }

            var exsistingCards = await client
                .From<Card>()
                .Select("position")
                .Filter("column_id", Supabase.Postgrest.Constants.Operator.Equals, request.ColumnId.ToString())
                .Filter("is_archived", Supabase.Postgrest.Constants.Operator.Equals, "false")
                .Order("position", Supabase.Postgrest.Constants.Ordering.Descending)
                .Limit(1)
                .Get();

            var maxPosition = exsistingCards.Models.FirstOrDefault()?.Position ?? 0;

            User? assignedUser = null;
            if (request.AssignedToId.HasValue)
            {
                assignedUser = await client
                    .From<User>()
                    .Select("id,username")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.AssignedToId.Value.ToString())
                    .Single();

                if(assignedUser == null)
                {
                    return Result.Failure<CardResponse>("Assigned user not found");
                }
            }

            var creator = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                .Single();

            var card = new Card
            {
                Id = Guid.NewGuid(),
                BoardId = request.BoardId,
                ColumnId = request.ColumnId,
                Title = request.Title,
                Description = request.Description,
                Position = maxPosition + 1,
                Priority = request.Priority,
                Status = "active",
                DueDate = request.DueDate,
                StartDate = request.StartDate,
                CreatedById = request.UserId,
                AssignedToId = request.AssignedToId,
                CustomFields = new CardCustomFields(),
                Metadata = new CardMetadata(),
                IsArchived = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await client
                .From<Card>()
                .Insert(card);

            var createdCard = insertResponse.Models.FirstOrDefault();

            if (createdCard == null)
            {
                return Result.Failure<CardResponse>("Failed to create card");
            }

            var response = new CardResponse(
                createdCard.Id,
                createdCard.BoardId,
                createdCard.ColumnId,
                column.Name,
                createdCard.Title,
                createdCard.Description,
                createdCard.Position,
                createdCard.Priority,
                createdCard.Status,
                createdCard.DueDate,
                createdCard.StartDate,
                createdCard.CompletedAt,
                createdCard.CreatedById,
                creator?.Username ?? "Unknown",
                createdCard.AssignedToId,
                assignedUser?.Username,
                createdCard.IsArchived,
                createdCard.CreatedAt,
                createdCard.UpdatedAt
            );

            return Result.Success(response);
        }
    }
    
}
