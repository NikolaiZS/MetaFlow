using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Tags;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.Tags.CreateTag
{
    public class CreateTagHandler : IRequestHandler<CreateTagCommand, Result<TagResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public CreateTagHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<TagResponse>> Handle(
            CreateTagCommand request,
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
                return Result.Failure<TagResponse>("Board not found");
            }

            if (board.OwnerId != request.UserId)
            {
                var member = await client
                    .From<BoardMember>()
                    .Select("id,role")
                    .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                    .Single();

                if (member == null || member.Role == "viewer")
                {
                    return Result.Failure<TagResponse>("Only board owner, admin, or member can create tags");
                }
            }

            var existingTag = await client
                .From<Tag>()
                .Select("id")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Filter("name", Supabase.Postgrest.Constants.Operator.Equals, request.Name)
                .Single();

            if (existingTag != null)
            {
                return Result.Failure<TagResponse>("Tag with this name already exists on this board");
            }

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                BoardId = request.BoardId,
                Name = request.Name,
                Color = request.Color,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await client
                .From<Tag>()
                .Insert(tag);

            var createdTag = insertResponse.Models.FirstOrDefault();

            if (createdTag == null)
            {
                return Result.Failure<TagResponse>("Failed to create tag");
            }

            var response = new TagResponse(
                createdTag.Id,
                createdTag.BoardId,
                createdTag.Name,
                createdTag.Color,
                createdTag.CreatedAt
            );

            return Result.Success(response);
        }
    }
}
