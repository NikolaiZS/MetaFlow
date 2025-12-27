using MediatR;
using MetaFlow.Api.Common;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Boards.DeleteBoard
{
    public class DeleteBoardHandler : IRequestHandler<DeleteBoardCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public DeleteBoardHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            DeleteBoardCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<bool>("Board not found");
            }

            if (board.OwnerId != request.UserId)
            {
                return Result.Failure<bool>("Only board owner can delete");
            }

            await client
                .From<Board>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Delete();

            return Result.Success(true);
        }
    }


}
