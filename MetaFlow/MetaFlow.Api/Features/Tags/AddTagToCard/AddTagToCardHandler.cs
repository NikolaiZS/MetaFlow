using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Tags.AddTagToCard
{
    public class AddTagToCardHandler : IRequestHandler<AddTagToCardCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public AddTagToCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            AddTagToCardCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var card = await client
                .From<Card>()
                .Select("id,board_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Single();

            if (card == null)
            {
                return Result.Failure<bool>("Card not found");
            }

            var tag = await client
                .From<Tag>()
                .Select("id,board_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.TagId.ToString())
                .Single();

            if (tag == null)
            {
                return Result.Failure<bool>("Tag not found");
            }

            if (tag.BoardId != card.BoardId)
            {
                return Result.Failure<bool>("Tag does not belong to the same board as the card");
            }

            var board = await client
                .From<Board>()
                .Select("id,owner_id,is_public")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<bool>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<bool>("Access denied");
            }

            var existingCardTag = await client
                .From<CardTag>()
                .Select("id")
                .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Filter("tag_id", Supabase.Postgrest.Constants.Operator.Equals, request.TagId.ToString())
                .Single();

            if (existingCardTag != null)
            {
                return Result.Failure<bool>("Tag is already added to this card");
            }

            var cardTag = new CardTag
            {
                Id = Guid.NewGuid(),
                CardId = request.CardId,
                TagId = request.TagId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await client
                .From<CardTag>()
                .Insert(cardTag);

            if (insertResponse.Models.Count == 0)
            {
                return Result.Failure<bool>("Failed to add tag to card");
            }

            return Result.Success(true);
        }
    }
}
