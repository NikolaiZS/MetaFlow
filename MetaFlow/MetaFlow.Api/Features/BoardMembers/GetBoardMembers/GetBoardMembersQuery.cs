using MetaFlow.Api.Common;
using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.BoardMembers;

namespace MetaFlow.Api.Features.BoardMembers.GetBoardMembers
{
    public record GetBoardMembersQuery(
    Guid BoardId,
    Guid UserId
    ) : IQuery<List<BoardMemberResponse>>;
}
