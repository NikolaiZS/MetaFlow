using MetaFlow.Api.Common;
using MetaFlow.Contracts.Boards;

namespace MetaFlow.Api.Features.Boards.GetBoards
{
    public record GetBoardsQuery(
    Guid UserId,
    bool IncludeArchived = false
) : IQuery<List<BoardListResponse>>;
}