using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.BoardMembers;
using MetaFlow.Domain.Entities;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.BoardMembers.GetBoardMembers
{
    public class GetBoardMembersHandler : IRequestHandler<GetBoardMembersQuery, Result<List<BoardMemberResponse>>>
    {
        private readonly SupabaseService _supabaseService;

        public GetBoardMembersHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<List<BoardMemberResponse>>> Handle(
            GetBoardMembersQuery request,
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
                return Result.Failure<List<BoardMemberResponse>>("Board not found");
            }

            if (board.OwnerId != request.UserId && !board.IsPublic)
            {
                return Result.Failure<List<BoardMemberResponse>>("Access denied");
            }

            var membersResponse = await client
                .From<BoardMember>()
                .Select("id,board_id,user_id,role,joined_at,invited_by")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Order("joined_at", Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            var members = membersResponse.Models;

            if (members.Count == 0)
            {
                return Result.Success(new List<BoardMemberResponse>());
            }

            var userIds = members.Select(m => m.UserId)
                .Concat(members.Where(m => m.InvitedBy.HasValue).Select(m => m.InvitedBy!.Value))
                .Distinct()
                .ToList();

            var users = new Dictionary<Guid, (string username, string? fullName, string? avatarUrl)>();
            foreach (var userId in userIds)
            {
                var user = await client
                    .From<User>()
                    .Select("id,username,full_name,avatar_url")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, userId.ToString())
                    .Single();

                if (user != null)
                {
                    users[userId] = (user.Username, user.FullName, user.AvatarUrl);
                }
            }

            var response = members.Select(m =>
            {
                var userInfo = users.GetValueOrDefault(m.UserId, ("Unknown", null, null));
                var inviterInfo = m.InvitedBy.HasValue
                    ? users.GetValueOrDefault(m.InvitedBy.Value, ("Unknown", null, null))
                    : (null, null, null);

                return new BoardMemberResponse(
                    m.Id,
                    m.BoardId,
                    m.UserId,
                    userInfo.username,
                    userInfo.fullName,
                    userInfo.avatarUrl,
                    m.Role,
                    m.JoinedAt,
                    m.InvitedBy,
                    inviterInfo.Item1
                );
            }).ToList();

            return Result.Success(response);
        }
    }
}
