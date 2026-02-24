using MediatR;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.BoardMembers;
using MetaFlow.Domain.Entities;
using MetaFlow.Domain.Models;
using MetaFlow.Infrastructure.Services;

namespace MetaFlow.Api.Features.BoardMembers.InviteMember
{
    public class InviteMemberHandler : IRequestHandler<InviteMemberCommand, Result<BoardMemberResponse>>
    {
        private readonly SupabaseService _supabaseService;

        public InviteMemberHandler(SupabaseService supabaseService)
        {
            _supabaseService = supabaseService;
        }

        public async Task<Result<BoardMemberResponse>> Handle(
            InviteMemberCommand request,
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

            if (board.OwnerId != request.InvitedById)
            {
                var inviterMember = await client
                    .From<BoardMember>()
                    .Select("id,role")
                    .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                    .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, request.InvitedById.ToString())
                    .Single();

                if (inviterMember == null || inviterMember.Role != "admin")
                {
                    return Result.Failure<BoardMemberResponse>("Only board owner or admin can invite members");
                }
            }

            var userToInvite = await client
                .From<User>()
                .Select("id,username,full_name,avatar_url")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                .Single();

            if (userToInvite == null)
            {
                return Result.Failure<BoardMemberResponse>("User to invite not found");
            }

            var existingMember = await client
                .From<BoardMember>()
                .Select("id")
                .Filter("board_id", Supabase.Postgrest.Constants.Operator.Equals, request.BoardId.ToString())
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, request.UserId.ToString())
                .Single();

            if (existingMember != null)
            {
                return Result.Failure<BoardMemberResponse>("User is already a member of this board");
            }

            var inviter = await client
                .From<User>()
                .Select("id,username")
                .Filter("id", Supabase.Postgrest.Constants.Operator.Equals, request.InvitedById.ToString())
                .Single();

            var boardMember = new BoardMember
            {
                Id = Guid.NewGuid(),
                BoardId = request.BoardId,
                UserId = request.UserId,
                Role = request.Role,
                Permissions = GetDefaultPermissions(request.Role),
                JoinedAt = DateTime.UtcNow,
                InvitedBy = request.InvitedById,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var insertResponse = await client
                .From<BoardMember>()
                .Insert(boardMember);

            var createdMember = insertResponse.Models.FirstOrDefault();

            if (createdMember == null)
            {
                return Result.Failure<BoardMemberResponse>("Failed to invite member");
            }

            var response = new BoardMemberResponse(
                createdMember.Id,
                createdMember.BoardId,
                createdMember.UserId,
                userToInvite.Username,
                userToInvite.FullName,
                userToInvite.AvatarUrl,
                createdMember.Role,
                createdMember.JoinedAt,
                createdMember.InvitedBy,
                inviter?.Username
            );

            return Result.Success(response);
        }

        private static MemberPermissions GetDefaultPermissions(string role)
        {
            return role switch
            {
                "owner" => new MemberPermissions
                {
                    CanCreateCards = true,
                    CanEditCards = true,
                    CanDeleteCards = true,
                    CanMoveCards = true,
                    CanManageColumns = true,
                    CanInviteMembers = true,
                    CanManageBoard = true,
                    CanArchiveBoard = true
                },
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
