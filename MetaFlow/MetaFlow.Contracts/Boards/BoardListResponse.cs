namespace MetaFlow.Contracts.Boards
{
    public record BoardListResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid OwnerId,
    string OwnerUsername,
    string MethodologyName,
    bool IsPublic,
    bool IsArchived,
    DateTime UpdatedAt
);
}