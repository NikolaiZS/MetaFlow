namespace MetaFlow.Contracts.Cards
{
    public record UpdateCardRequest(
        string? Title,
        string? Description,
        string? Priority,
        string? Status,
        DateTime? DueDate,
        DateTime? StartTime,
        Guid? AssignedToId
        );
}