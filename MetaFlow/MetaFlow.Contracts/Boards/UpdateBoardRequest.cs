namespace MetaFlow.Contracts.Boards
{
    public record UpdateBoardRequest(
    string? Name,
    string? Description,
    bool? IsPublic,
    bool? IsArchived
);
}