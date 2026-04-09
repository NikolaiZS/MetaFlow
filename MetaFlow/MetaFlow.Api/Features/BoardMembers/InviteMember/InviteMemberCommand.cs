using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.BoardMembers;


namespace MetaFlow.Api.Features.BoardMembers.InviteMember
{
    public record InviteMemberCommand(
    Guid BoardId,
    Guid UserId,
    string Role,
    Guid InvitedById
    ) : ICommand<BoardMemberResponse>;
}
