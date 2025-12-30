namespace MetaFlow.Contracts.Cards
{
    public record CardListResponse(
        Guid Id,
        string Title,
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