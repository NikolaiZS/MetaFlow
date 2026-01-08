using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.BoardMembers.RemoveMember
{
    public record RemoveMemberCommand(
    Guid BoardId,
    Guid MemberId,
    Guid UserId
    ) : ICommand<bool>;
}
