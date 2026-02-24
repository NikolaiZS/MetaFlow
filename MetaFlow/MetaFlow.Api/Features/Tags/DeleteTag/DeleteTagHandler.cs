using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Tags.DeleteTag
{
    public class DeleteTagHandler : IRequestHandler<DeleteTagCommand, Result<bool>>
    {
        private readonly SupabaseService _supabaseService;

        public DeleteTagHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<bool>> Handle(
            DeleteTagCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var tag = await client
                .From<Tag>()
                .Select("id,board_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.TagId.ToString())
                .Single();

            if (tag == null)
            {
                return Result.Failure<bool>("Tag not found");
            }

            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, tag.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<bool>("Board not found");
            }

            if (board.OwnerId != request.UserId)
            {
                var member = await client
                    .From<BoardMember>()
                    .Select("id,role")
                    .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, tag.BoardId.ToString())
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                    .Single();

                if (member == null || member.Role == "viewer")
                {
                    return Result.Failure<bool>("Only board owner, admin, or member can delete tags");
                }
            }

            await client
                .From<Tag>()
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.TagId.ToString())
                .Delete();

            return Result.Success(true);
        }
    }
}
