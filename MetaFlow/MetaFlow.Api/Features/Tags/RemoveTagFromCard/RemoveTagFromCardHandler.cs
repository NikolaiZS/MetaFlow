using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Tags.RemoveTagFromCard
{
    public class RemoveTagFromCardHandler : IRequestHandler<RemoveTagFromCardCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public RemoveTagFromCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            RemoveTagFromCardCommand request,
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

            var cardTag = await client
                .From<CardTag>()
                .Select("id")
                .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Filter("tag_id", Supabase.Postgrest.Constants.Operator.Equals, request.TagId.ToString())
                .Single();

            if (cardTag == null)
            {
                return Result.Failure<bool>("Tag is not associated with this card");
            }

            await client
                .From<CardTag>()
                .Filter("card_id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Filter("tag_id", Supabase.Postgrest.Constants.Operator.Equals, request.TagId.ToString())
                .Delete();

            return Result.Success(true);
        }
    }
}
