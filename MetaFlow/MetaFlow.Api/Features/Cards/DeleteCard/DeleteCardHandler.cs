using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Cards.DeleteCard
{
    public class DeleteCardHandler : IRequestHandler<DeleteCardCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public DeleteCardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            DeleteCardCommand request,
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
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, card.BoardId.ToString())
                .Single();

            if (board == null || board.OwnerId != request.UserId)
            {
                return Result.Failure<bool>("Only board owner can delete cards");
            }

            await client
                .From<Card>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.CardId.ToString())
                .Delete();

            return Result.Success(true);
        }
    }
}