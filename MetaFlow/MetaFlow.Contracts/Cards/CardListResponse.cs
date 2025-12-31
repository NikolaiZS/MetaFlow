namespace MetaFlow.Contracts.Cards
{
    public record CardListResponse(
        Guid Id,
        string Title,
        string? Description,
        Guid ColumnId,
        double Position,
        string Priority,
        string Status,
        DateTime? DueDate,
        Guid? AssignedToId,
        string? AssignedToUsername,
        int CommentsCount,
        int AttachmentCount,
        DateTime UpdatedAt
        );
}