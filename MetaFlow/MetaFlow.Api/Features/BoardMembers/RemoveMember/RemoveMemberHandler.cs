using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;


namespace MetaFlow.Api.Features.BoardMembers.RemoveMember
{
    public class RemoveMemberHandler : IRequestHandler<RemoveMemberCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public RemoveMemberHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            RemoveMemberCommand request,
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

            var member = await client
                .From<BoardMember>()
                .Select("id,board_id,user_id,role")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.MemberId.ToString())
                .Single();

            if (member == null || member.BoardId != request.BoardId)
            {
                return Result.Failure<bool>("Member not found");
            }

            if (member.UserId == board.OwnerId)
            {
                return Result.Failure<bool>("Cannot remove board owner");
            }

            if (member.UserId != request.UserId)
            {
                if (board.OwnerId != request.UserId)
                {
                    var userMember = await client
                        .From<BoardMember>()
                        .Select("id,role")
                        .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                        .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                        .Single();

                    if (userMember == null || userMember.Role != "admin")
                    {
                        return Result.Failure<bool>("Only board owner, admin, or the member themselves can remove membership");
                    }
                }
            }

            await client
                .From<BoardMember>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.MemberId.ToString())
                .Delete();

            return Result.Success(true);
        }
    }
}
