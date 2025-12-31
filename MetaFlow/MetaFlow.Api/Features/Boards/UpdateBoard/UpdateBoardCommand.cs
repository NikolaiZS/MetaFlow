using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Boards;

namespace MetaFlow.Api.Features.Boards.UpdateBoard
{
    public record UpdateBoardCommand(
    Guid BoardId,
    Guid UserId,
    string? Name,
    string? Description,
    bool? IsPublic,
    bool? IsArchived
) : ICommand<BoardResponse>;
}