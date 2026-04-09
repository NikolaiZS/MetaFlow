using MetaFlow.Api.Common;
using MetaFlow.Contracts.Boards;

namespace MetaFlow.Api.Features.Boards.GetBoard
{
    public record GetBoardQuery(
    Guid BoardId,
    Guid UserId
) : IQuery<BoardResponse>;
}