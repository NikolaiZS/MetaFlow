using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Cards.ArchiveCard
{
    public class ArchiveCardHandler : IRequestHandler<ArchiveCardCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public ArchiveCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            ArchiveCardCommand request,
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
                return Result.Failure<bool>("Card not found");
            }

            // Verify board access
            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null || board.OwnerId != request.UserId)
            {
                return Result.Failure<bool>("Only board owner can archive cards");
            }

            // Update card
            card.IsArchived = request.Archive;
            card.UpdatedAt = DateTime.UtcNow;

            await client
                .From<Card>()
                .Update(card);

            return Result.Success(true);
        }
    }
}
