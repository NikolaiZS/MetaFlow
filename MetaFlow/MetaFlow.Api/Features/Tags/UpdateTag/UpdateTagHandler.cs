using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Tags;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Tags.UpdateTag
{
    public class UpdateTagHandler : IRequestHandler<UpdateTagCommand, Result<TagResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public UpdateTagHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<TagResponse>> Handle(
            UpdateTagCommand request,
            CancellationToken cancellationToken)
        {
            var client = _supabaseService.GetClient();

            var tag = await client
                .From<Tag>()
                .Select("id,board_id,name,color,created_at,updated_at")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.TagId.ToString())
                .Single();

            if (tag == null)
            {
                return Result.Failure<TagResponse>("Tag not found");
            }

            var board = await client
                .From<Board>()
                .Select("id,owner_id")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, tag.BoardId.ToString())
                .Single();

            if (board == null)
            {
                return Result.Failure<TagResponse>("Board not found");
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
                    return Result.Failure<TagResponse>("Only board owner, admin, or member can update tags");
                }
            }

            if (request.Name != null) tag.Name = request.Name;
            if (request.Color != null) tag.Color = request.Color;
            tag.UpdatedAt = DateTime.UtcNow;

            await client
                .From<Tag>()
                .Update(tag);

            var response = new TagResponse(
                tag.Id,
                tag.BoardId,
                tag.Name,
                tag.Color,
                tag.CreatedAt
            );

            return Result.Success(response);
        }
    }
}
