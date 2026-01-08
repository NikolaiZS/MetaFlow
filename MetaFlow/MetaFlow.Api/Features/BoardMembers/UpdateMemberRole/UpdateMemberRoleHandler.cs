using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.BoardMembers;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.BoardMembers.UpdateMemberRole
{
    public class UpdateMemberRoleHandler : IRequestHandler<UpdateMemberRoleCommand, Result<BoardMemberResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public UpdateMemberRoleHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<BoardMemberResponse>> Handle(
            UpdateMemberRoleCommand request,
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
                return Result.Failure<BoardMemberResponse>("Board not found");
            }

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
                    return Result.Failure<BoardMemberResponse>("Only board owner or admin can update member roles");
                }
            }

            var member = await client
                .From<BoardMember>()
                .Select("id,board_id,user_id,role,joined_at,invited_by")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.MemberId.ToString())
                .Single();

            if (member == null || member.BoardId != request.BoardId)
            {
                return Result.Failure<BoardMemberResponse>("Member not found");
            }

            if (member.UserId == board.OwnerId)
            {
                return Result.Failure<BoardMemberResponse>("Cannot change board owner's role");
            }

            member.Role = request.Role;
            member.Permissions = GetDefaultPermissions(request.Role);
            member.UpdatedAt = DateTime.UtcNow;

            await client
                .From<BoardMember>()
                .Update(member);

            var user = await client
                .From<User>()
                .Select("id,username,full_name,avatar_url")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, member.UserId.ToString())
                .Single();

            User? inviter = null;
            if (member.InvitedBy.HasValue)
            {
                inviter = await client
                    .From<User>()
                    .Select("id,username")
                    .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, member.InvitedBy.Value.ToString())
                    .Single();
            }

            var response = new BoardMemberResponse(
                member.Id,
                member.BoardId,
                member.UserId,
                user?.Username ?? "Unknown",
                user?.FullName,
                user?.AvatarUrl,
                member.Role,
                member.JoinedAt,
                member.InvitedBy,
                inviter?.Username
            );

            return Result.Success(response);
        }

        private static MemberPermissions GetDefaultPermissions(string role)
        {
            return role switch
            {
                "admin" => new MemberPermissions
                {
                    CanCreateCards = true,
                    CanEditCards = true,
                    CanDeleteCards = true,
                    CanMoveCards = true,
                    CanManageColumns = true,
                    CanInviteMembers = true,
                    CanManageBoard = false,
                    CanArchiveBoard = false
                },
                "member" => new MemberPermissions
                {
                    CanCreateCards = true,
                    CanEditCards = true,
                    CanDeleteCards = false,
                    CanMoveCards = true,
                    CanManageColumns = false,
                    CanInviteMembers = false,
                    CanManageBoard = false,
                    CanArchiveBoard = false
                },
                "viewer" => new MemberPermissions
                {
                    CanCreateCards = false,
                    CanEditCards = false,
                    CanDeleteCards = false,
                    CanMoveCards = false,
                    CanManageColumns = false,
                    CanInviteMembers = false,
                    CanManageBoard = false,
                    CanArchiveBoard = false
                },
                _ => new MemberPermissions()
            };
        }
    }
}
