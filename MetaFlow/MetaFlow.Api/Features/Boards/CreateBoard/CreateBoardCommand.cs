using MetaFlow.Api.Common.Abstractions;
using MetaFlow.Contracts.Boards;

namespace MetaFlow.Api.Features.Boards.CreateBoard
{
    public record CreateBoardCommand(
    string Name,
    string? Description,
    Guid MethodologyPresetId,
    bool IsPublic,
    bool IsTemplate,
    Guid UserId
) : ICommand<BoardResponse>;
}