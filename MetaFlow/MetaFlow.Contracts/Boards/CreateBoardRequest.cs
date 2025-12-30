namespace MetaFlow.Contracts.Boards
{
    public record CreateBoardRequest(
    string Name,
    string? Description,
    Guid MethodologyPresetId,
    bool IsPublic = false,
    bool IsTemplate = false
);
}