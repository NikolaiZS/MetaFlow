using MetaFlow.Api.Common.Abstractions;

namespace MetaFlow.Api.Features.Boards.DeleteBoard
{
    public record DeleteBoardCommand(
    Guid BoardId,
    Guid UserId
) : ICommand<bool>;
}