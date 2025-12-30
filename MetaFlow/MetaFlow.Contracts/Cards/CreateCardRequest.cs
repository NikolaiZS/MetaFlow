namespace MetaFlow.Contracts.Cards
{
    public record CreateCardRequest(
        string Title,
        string? Description,
        Guid ColumnId,
        string Priority = "medium",
        DateTime? DueDate = null,
        DateTime? StartDate = null,
        Guid? AssignedToId = null
        );
}