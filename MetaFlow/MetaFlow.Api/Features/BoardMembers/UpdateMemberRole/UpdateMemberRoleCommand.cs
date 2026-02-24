using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.BoardMembers;

namespace MetaFlow.Api.Features.BoardMembers.UpdateMemberRole
{
    public record UpdateMemberRoleCommand(
    Guid BoardId,
    Guid MemberId,
    string Role,
    Guid UserId
    ) : ICommand<BoardMemberResponse>;
}
