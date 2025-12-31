namespace MetaFlow.Contracts.Boards
{
    public record BoardResponse(
    Guid Id,
    string Name,
    string? Description,
    Guid OwnerId,
    string OwnerUsername,
    Guid MethodologyPresetId,
    string MethodologyName,
    bool IsPublic,
    bool IsTemplate,
    bool IsArchived,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
}